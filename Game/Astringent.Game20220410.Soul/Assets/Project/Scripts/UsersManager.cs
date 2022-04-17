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
        readonly System.Collections.Generic.List<User> _Users;
        private readonly EntitesKeeper _Keeper;

        public UsersManager()
        {
            _Keeper = new EntitesKeeper();
            _Users  = new System.Collections.Generic.List<User>();
        }
        
        void Start()
        {                     
        }

        
        void Update()
        {
            foreach (var user in _Users)
            {
                user.Update ();
            }
        }

        
      
        public void BinderEnter(Regulus.Remote.IBinder binder)
        {
           
           _Users.Add(new User(_Keeper , binder));


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
