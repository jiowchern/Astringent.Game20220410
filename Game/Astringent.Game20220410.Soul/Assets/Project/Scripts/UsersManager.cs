using Unity.Entities;
using Unity.Transforms;

using Unity.Rendering;

using UnityEngine;
using Astringent.Game20220410.Sources;

using Unity.Mathematics;

namespace Astringent.Game20220410.Scripts
{
    
    public class UsersManager : MonoBehaviour 
    {
        

        
        

        readonly IdDispenser _IdDispenser;

        readonly System.Collections.Generic.List<User> _Users;
        public UsersManager()
        {
            _IdDispenser = new IdDispenser();
            _Users  = new System.Collections.Generic.List<User>();
        }
        
        void Start()
        {                     
        }

        
        void Update()
        {
            foreach (var user in _Users)
            {
                user.SyncStates();
            }
        }

        internal Entity Spawn(long id)
        {
            var entityManager = Service.GetWorld().EntityManager;
            var entity = entityManager.Instantiate(SoulPrototypesProvider.ActorEntity);
            entityManager.SetComponentData<Dots.ActorAttributes>(entity, new Protocol.Attributes {  Id = id, Speed = 1f } );
            return entity;


        }

      
        public void BinderEnter(Regulus.Remote.IBinder binder)
        {
            var id = _IdDispenser.Dispatch(binder);
            _Users.Add(new User(id , Spawn(id), binder));


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
