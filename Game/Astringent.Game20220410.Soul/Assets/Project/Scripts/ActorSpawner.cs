using Unity.Entities;
using Unity.Transforms;

using Unity.Rendering;

using UnityEngine;
using Astringent.Game20220410.Sources;

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
                                      typeof(Dots.MoveingState),
                                      typeof(Translation),
                                      typeof(RenderMesh),
                                      typeof(LocalToWorld),
                                      typeof(RenderBounds));
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        internal Entity Spawn()
        {
            var entityManager = Service.GetWorld().EntityManager;
            var entity = entityManager.CreateEntity(_EntityArchetype);
            var meshFilter = Actor.GetComponent<UnityEngine.MeshFilter>();
            var meshRender = Actor.GetComponent<UnityEngine.MeshRenderer>();

            entityManager.SetSharedComponentData(entity, new RenderMesh { mesh = meshFilter.sharedMesh , material = meshRender.sharedMaterial} );            
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
                using (user) ;
            }
            _Users.RemoveAll(x => x.Binder == binder);


        }
    }

}
