using Unity.Entities;
using Unity.Transforms;

using Unity.Rendering;

using UnityEngine;
using Astringent.Game20220410.Sources;
using Astringent.Game20220410.Protocol;
using Unity.Jobs;
using Unity.Mathematics;

namespace Astringent.Game20220410.Scripts
{
    
    public class ActorSpawner : MonoBehaviour
    {
        private EntityArchetype _EntityArchetype;

        public GameObject Actor;
        

        readonly System.Collections.Generic.List<User> _Users;
        public ActorSpawner()
        {
            _Users  = new System.Collections.Generic.List<User>();
        }
        // Start is called before the first frame update
        void Start()
        {            
            var entityManager = Service.GetWorld().EntityManager;
            _EntityArchetype = entityManager.CreateArchetype(
                                      typeof(Dots.Direction),
                                      typeof(MoveingState),
                                      typeof(ActorAttributes),
                                      typeof(Translation),
                                      typeof(RenderMesh),
                                      typeof(LocalToWorld),
                                      typeof(RenderBounds));
        }

        // Update is called once per frame
        void Update()
        {
            foreach (var user in _Users)
            {
                user.SyncStates();
            }
        }

        internal Entity Spawn()
        {
            var entityManager = Service.GetWorld().EntityManager;
            var entity = entityManager.CreateEntity(_EntityArchetype);
            var meshFilter = Actor.GetComponent<UnityEngine.MeshFilter>();
            var meshRender = Actor.GetComponent<UnityEngine.MeshRenderer>();

            

            entityManager.SetComponentData(entity, new ActorAttributes { Direction = 0f , Speed = 1f});


            entityManager.SetComponentData(entity, new LocalToWorld(){
                Value = new float4x4(rotation: quaternion.identity, translation: new float3(0, 0, 0))
            });
            entityManager.SetSharedComponentData(entity, new RenderMesh()
            {
                layerMask = 1,
                          mesh = meshFilter.sharedMesh,
                            material = meshRender.sharedMaterial

            });
            entityManager.SetComponentData(entity, new RenderBounds()
            {
                Value = new AABB()
                {
                    Center = new float3(0, 0, 0),
                    Extents = new float3(0.5f, 0.5f, 0.5f)
                }
            });
            return entity;


        }

      
        public void BinderEnter(Regulus.Remote.IBinder binder)
        {

            _Users.Add(new User(Spawn(), binder));


        }

        public void BinderLeave(Regulus.Remote.IBinder binder)
        {
            
            foreach (var user in _Users)
            {
                if (user.Binder != binder)
                    continue;
                using (user)
                {

                }
            }
            _Users.RemoveAll(x => x.Binder == binder);


        }
    }

}
