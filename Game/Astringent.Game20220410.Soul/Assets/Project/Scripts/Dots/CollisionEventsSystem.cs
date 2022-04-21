using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace Astringent.Game20220410.Dots
{
}
namespace Astringent.Game20220410.Dots.Systems
{




    public partial class CollisionEventsSystem : SystemBase
    {
        BuildPhysicsWorld _buildPhysicsWorldSystem;
        StepPhysicsWorld _stepPhysicsWorldSystem;
        EntityQuery _collisionEventsBufferEntityQuery;

        // todo; can maybe optimize by checking if chunk has changed?
        [BurstCompile]
        struct CollisionEventsPreProcessJob : IJobChunk
        {
            public BufferTypeHandle<CollisionEventBufferElement> CollisionEventBufferType;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                BufferAccessor<CollisionEventBufferElement> collisionEventsBufferAccessor = chunk.GetBufferAccessor(CollisionEventBufferType);

                for (int i = 0; i < chunk.Count; i++)
                {
                    DynamicBuffer<CollisionEventBufferElement> collisionEventsBuffer = collisionEventsBufferAccessor[i];

                    for (int j = collisionEventsBuffer.Length - 1; j >= 0; j--)
                    {
                        CollisionEventBufferElement collisionEventElement = collisionEventsBuffer[j];
                        collisionEventElement._isStale = true;
                        collisionEventsBuffer[j] = collisionEventElement;
                    }
                }
            }
        }

        [BurstCompile]
        struct CollisionEventsJob : ICollisionEventsJob
        {
            [ReadOnly]
            public PhysicsWorld PhysicsWorld;
            public BufferFromEntity<CollisionEventBufferElement> CollisionEventBufferFromEntity;
            [ReadOnly]
            public ComponentDataFromEntity<CollisionEventsReceiverProperties> CollisionEventsReceiverPropertiesFromEntity;

            public void Execute(CollisionEvent collisionEvent)
            {
                UnityEngine.Debug.Log("public void Execute(CollisionEvent collisionEvent)");
                CollisionEvent.Details collisionEventDetails = default;

                bool AHasDetails = false;
                bool BHasDetails = false;
                
                if (CollisionEventsReceiverPropertiesFromEntity.HasComponent(collisionEvent.EntityA))
                {
                    AHasDetails = CollisionEventsReceiverPropertiesFromEntity[collisionEvent.EntityA].UsesCollisionDetails;
                }
                if (CollisionEventsReceiverPropertiesFromEntity.HasComponent(collisionEvent.EntityB))
                {
                    BHasDetails = CollisionEventsReceiverPropertiesFromEntity[collisionEvent.EntityB].UsesCollisionDetails;
                }

                if (AHasDetails || BHasDetails)
                {
                    collisionEventDetails = collisionEvent.CalculateDetails(ref PhysicsWorld);
                }

                if (CollisionEventBufferFromEntity.HasComponent(collisionEvent.EntityA))
                {
                    ProcessForEntity(collisionEvent.EntityA, collisionEvent.EntityB, collisionEvent.Normal, AHasDetails, collisionEventDetails);
                }
                if (CollisionEventBufferFromEntity.HasComponent(collisionEvent.EntityB))
                {
                    ProcessForEntity(collisionEvent.EntityB, collisionEvent.EntityA, collisionEvent.Normal, BHasDetails, collisionEventDetails);
                }
            }

            private void ProcessForEntity(Entity entity, Entity otherEntity, float3 normal, bool hasDetails, CollisionEvent.Details collisionEventDetails)
            {
                DynamicBuffer<CollisionEventBufferElement> collisionEventBuffer = CollisionEventBufferFromEntity[entity];

                bool foundMatch = false;
                for (int i = 0; i < collisionEventBuffer.Length; i++)
                {
                    CollisionEventBufferElement collisionEvent = collisionEventBuffer[i];

                    // If entity is already there, update to Stay
                    if (collisionEvent.Entity == otherEntity)
                    {
                        foundMatch = true;
                        collisionEvent.Normal = normal;
                        collisionEvent.HasCollisionDetails = hasDetails;
                        collisionEvent.AverageContactPointPosition = collisionEventDetails.AverageContactPointPosition;
                        collisionEvent.EstimatedImpulse = collisionEventDetails.EstimatedImpulse;
                        collisionEvent.State = PhysicsEventState.Stay;
                        collisionEvent._isStale = false;
                        collisionEventBuffer[i] = collisionEvent;

                        break;
                    }
                }

                // If it's a new entity, add as Enter
                if (!foundMatch)
                {
                    collisionEventBuffer.Add(new CollisionEventBufferElement
                    {
                        Entity = otherEntity,
                        Normal = normal,
                        HasCollisionDetails = hasDetails,
                        AverageContactPointPosition = collisionEventDetails.AverageContactPointPosition,
                        EstimatedImpulse = collisionEventDetails.EstimatedImpulse,
                        State = PhysicsEventState.Enter,
                        _isStale = false,
                    });
                }
            }
        }

        [BurstCompile]
        struct CollisionEventsPostProcessJob : IJobChunk
        {
            public BufferTypeHandle<CollisionEventBufferElement> CollisionEventBufferType;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                if (chunk.Has(CollisionEventBufferType))
                {
                    BufferAccessor<CollisionEventBufferElement> collisionEventsBufferAccessor = chunk.GetBufferAccessor(CollisionEventBufferType);

                    for (int i = 0; i < chunk.Count; i++)
                    {
                        DynamicBuffer<CollisionEventBufferElement> collisionEventsBuffer = collisionEventsBufferAccessor[i];

                        for (int j = collisionEventsBuffer.Length - 1; j >= 0; j--)
                        {
                            CollisionEventBufferElement collisionEvent = collisionEventsBuffer[j];

                            if (collisionEvent._isStale)
                            {
                                if (collisionEvent.State == PhysicsEventState.Exit)
                                {
                                    collisionEventsBuffer.RemoveAt(j);
                                }
                                else
                                {
                                    collisionEvent.State = PhysicsEventState.Exit;
                                    collisionEventsBuffer[j] = collisionEvent;
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override void OnCreate()
        {
            _buildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
            _stepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();

            EntityQueryDesc queryDesc = new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                typeof(PhysicsCollider),
                typeof(CollisionEventBufferElement),
                typeof(CollisionEventsReceiverProperties),
                },
            };

            _collisionEventsBufferEntityQuery = GetEntityQuery(queryDesc);
        }

        protected override void OnUpdate()
        {
            Dependency = new CollisionEventsPreProcessJob
            {
                CollisionEventBufferType = GetBufferTypeHandle<CollisionEventBufferElement>(),
            }.ScheduleParallel(_collisionEventsBufferEntityQuery, Dependency);
            
            Dependency = new CollisionEventsJob
            {
                PhysicsWorld = _buildPhysicsWorldSystem.PhysicsWorld,
                CollisionEventBufferFromEntity = GetBufferFromEntity<CollisionEventBufferElement>(),
                CollisionEventsReceiverPropertiesFromEntity = GetComponentDataFromEntity<CollisionEventsReceiverProperties>(true),
            }.Schedule(_stepPhysicsWorldSystem.Simulation,  Dependency);
            
            Dependency = new CollisionEventsPostProcessJob
            {
                CollisionEventBufferType = GetBufferTypeHandle<CollisionEventBufferElement>(),
            }.ScheduleParallel(_collisionEventsBufferEntityQuery, Dependency);
            Dependency.Complete();
        }
    }
}