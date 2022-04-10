using Regulus.Remote.Soul;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;



namespace Astringent.Game20220410.Scripts
{
    public class Entry : MonoBehaviour , Regulus.Remote.IEntry
    {

        int _IdProvider;
        readonly System.Collections.Concurrent.ConcurrentQueue<Regulus.Remote.IBinder> _AddBinders;
        private readonly List<Astringent.Game20220410.Sources.User> _Users;
        readonly System.Collections.Concurrent.ConcurrentQueue<Regulus.Remote.IBinder> _RemoveBinders;
        private IService _Service;
        readonly System.Collections.Generic.List<System.Action> _Closes ;
        public Entry()
        {
            _Closes = new List<System.Action>();
            _Users = new List<Astringent.Game20220410.Sources.User>();
            _RemoveBinders = new System.Collections.Concurrent.ConcurrentQueue<Regulus.Remote.IBinder>();
            _AddBinders = new System.Collections.Concurrent.ConcurrentQueue<Regulus.Remote.IBinder>();
        }
        void Regulus.Remote.IBinderProvider.AssignBinder(Regulus.Remote.IBinder binder)
        {
            binder.BreakEvent += ()=>_RemoveBinders.Enqueue (binder);
            _AddBinders.Enqueue(binder);
        }

        // Start is called before the first frame update
        void Start()
        {
            
            var listener = new Soul.Sources.Listener();

            
            
            {
                var tcp = new Regulus.Remote.Server.Tcp.Listener();
                listener.Add(tcp);
                tcp.Bind(53003);
                _Closes.Add(() => tcp.Close());
            }

            
            {
                /*var web = new Regulus.Remote.Server.Web.Listener();
                listener.Add(web);
                web.Bind($"http://*:{53003}/");
                Closes.Add(() => web.Close());*/
            }
            
            var protocol = Astringent.Game20220410.Protocol.Provider.Create();
            _Service = Regulus.Remote.Server.Provider.CreateService(this, protocol, listener);
        }

        // Update is called once per frame
        void Update()
        {
            Regulus.Remote.IBinder binder;
            while(_AddBinders.TryDequeue(out binder))
            {
                var id = ++_IdProvider;
                
                
                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                

                var entityArchetype = entityManager.CreateArchetype(                                            
                                           typeof(Dots.Direction),
                                           typeof(Dots.MoveingState),
                                           typeof(Translation)
                                           /*typeof(RenderMesh),
                                           typeof(LocalToWorld),
                                           typeof(RenderBounds)*/);

                Entity entity = entityManager.CreateEntity(entityArchetype);

                _Users.Add(new Astringent.Game20220410.Sources.User(entity, binder));
            }
            while (_RemoveBinders.TryDequeue(out binder))
            {
                var user = _Users.Find((user)=> user.Binder == binder);
                using (user)
                {
                    _Users.Remove(user);
                }
                
            }


            foreach (var user in _Users)
            {
                user.StateSample();
            }
        }

        void OnDestroy()
        {
            foreach (var item in _Closes)
            {
                item();
            }
            _Service.Dispose();
        }
    }

}
