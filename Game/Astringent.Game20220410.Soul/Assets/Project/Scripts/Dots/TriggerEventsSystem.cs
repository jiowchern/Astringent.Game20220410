using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace Astringent.Game20220410.Dots.Systems
{
    public partial class TriggerEventsSystem : SystemBase
    {
        StepPhysicsWorld _stepPhysicsWorldSystem;
        EntityQuery _triggerEventsBufferEntityQuery;

        // todo; can maybe optimize by checking if chunk has changed?
        [BurstCompile]
        struct TriggerEventsPreProcessJob : IJobChunk
        {
            
            public BufferTypeHandle<TriggerEventBufferElement> TriggerEventBufferType;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                
                BufferAccessor<TriggerEventBufferElement> triggerEventsBufferAccessor = chunk.GetBufferAccessor(TriggerEventBufferType);

                for (int i = 0; i < chunk.Count; i++)
                {
                    DynamicBuffer<TriggerEventBufferElement> triggerEventsBuffer = triggerEventsBufferAccessor[i];

                    for (int j = triggerEventsBuffer.Length - 1; j >= 0; j--)
                    {
                        TriggerEventBufferElement triggerEventElement = triggerEventsBuffer[j];
                        triggerEventElement._isStale = true;
                        triggerEventsBuffer[j] = triggerEventElement;
                    }
                }

            }
        }

        [BurstCompile]
        struct TriggerEventsJob : ITriggerEventsJob
        {
            public BufferFromEntity<TriggerEventBufferElement> TriggerEventBufferFromEntity;

            public void Execute(TriggerEvent triggerEvent)
            {
                
                ProcessForEntity(triggerEvent.EntityA, triggerEvent.EntityB);
                ProcessForEntity(triggerEvent.EntityB, triggerEvent.EntityA);

                
            }

            private void ProcessForEntity(Entity entity, Entity otherEntity)
            {
                
                if (TriggerEventBufferFromEntity.HasComponent(entity))
                {
                    DynamicBuffer<TriggerEventBufferElement> triggerEventBuffer = TriggerEventBufferFromEntity[entity];

                    bool foundMatch = false;
                    for (int i = 0; i < triggerEventBuffer.Length; i++)
                    {
                        TriggerEventBufferElement triggerEvent = triggerEventBuffer[i];

                        // If entity is already there, update to Stay
                        if (triggerEvent.Entity == otherEntity)
                        {
                            foundMatch = true;
                            triggerEvent.State = PhysicsEventState.Stay;
                            triggerEvent._isStale = false;
                            triggerEventBuffer[i] = triggerEvent;

                            break;
                        }
                    }

                    // If it's a new entity, add as Enter
                    if (!foundMatch)
                    {

                        UnityEngine.Debug.Log("PhysicsEventState.Enter");
                        triggerEventBuffer.Add(new TriggerEventBufferElement
                        {
                            Entity = otherEntity,
                            State = PhysicsEventState.Enter,
                            _isStale = false,
                        });
                    }
                }
            }
        }

        [BurstCompile]
        struct TriggerEventsPostProcessJob : IJobChunk
        {
            public BufferTypeHandle<TriggerEventBufferElement> TriggerEventBufferType;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                BufferAccessor<TriggerEventBufferElement> triggerEventsBufferAccessor = chunk.GetBufferAccessor(TriggerEventBufferType);

                for (int i = 0; i < chunk.Count; i++)
                {
                    DynamicBuffer<TriggerEventBufferElement> triggerEventsBuffer = triggerEventsBufferAccessor[i];

                    for (int j = triggerEventsBuffer.Length - 1; j >= 0; j--)
                    {
                        TriggerEventBufferElement triggerEvent = triggerEventsBuffer[j];

                        if (triggerEvent._isStale)
                        {
                            if (triggerEvent.State == PhysicsEventState.Exit)
                            {
                                triggerEventsBuffer.RemoveAt(j);
                            }
                            else
                            {
                                triggerEvent.State = PhysicsEventState.Exit;
                                triggerEventsBuffer[j] = triggerEvent;
                            }
                        }
                    }
                }

                

            }
        }

        protected override void OnCreate()
        {
            
            _stepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();

            EntityQueryDesc queryDesc = new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                typeof(PhysicsCollider),
                typeof(TriggerEventBufferElement),
                }
            };

            _triggerEventsBufferEntityQuery = GetEntityQuery(queryDesc);
        }

        protected override void OnUpdate()
        {


        
            Dependency = new TriggerEventsPreProcessJob
            {
                TriggerEventBufferType = GetBufferTypeHandle<TriggerEventBufferElement>(),
            }.ScheduleParallel(_triggerEventsBufferEntityQuery, Dependency);
            
            Dependency = new TriggerEventsJob
            {
                TriggerEventBufferFromEntity = GetBufferFromEntity<TriggerEventBufferElement>(),
            }.Schedule(_stepPhysicsWorldSystem.Simulation,  Dependency);

            Dependency = new TriggerEventsPostProcessJob
            {
                TriggerEventBufferType = GetBufferTypeHandle<TriggerEventBufferElement>(),
            }.ScheduleParallel(_triggerEventsBufferEntityQuery, Dependency);
        }
    }
}
