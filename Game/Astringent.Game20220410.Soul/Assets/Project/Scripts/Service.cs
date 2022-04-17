using Regulus.Remote.Soul;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

/// <summary>
/// UNITY_DISABLE_AUTOMATIC_SYSTEM_BOOTSTRAP
/// </summary>

namespace Astringent.Game20220410.Scripts
{
    public class Service : MonoBehaviour , Regulus.Remote.IEntry
    {
        public readonly static string WorldName = "server";
        
        public static World GetWorld()
        {
/*            foreach (var world in World.All)
            {
                if (world.Name != WorldName)
                    continue;
                return world;                
            }
            return new World(WorldName);*/
            
            return World.DefaultGameObjectInjectionWorld;
        }

        public int TcpPort;
        

        readonly System.Collections.Concurrent.ConcurrentQueue<Regulus.Remote.IBinder> _AddBinders;        
        readonly System.Collections.Concurrent.ConcurrentQueue<Regulus.Remote.IBinder> _RemoveBinders;
        
        private IService _Service;
        readonly System.Collections.Generic.List<System.Action> _Closes ;
        public Service()
        {
            _Closes = new List<System.Action>();
            
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
            var protocol = Astringent.Game20220410.Protocol.Provider.Create();
            var set = Regulus.Remote.Server.Provider.CreateTcpService(this, protocol);
            set.Listener.Bind(TcpPort);
            _Service = set.Service;
            _Closes.Add(() => set.Listener.Close());
        }
        

        // Update is called once per frame
        void Update()
        {
            Regulus.Remote.IBinder binder;
            while(_AddBinders.TryDequeue(out binder))
            {
                BinderEnterEvent.Invoke(binder);
            }
            while (_RemoveBinders.TryDequeue(out binder))
            {

                BinderLeaveEvent.Invoke(binder);
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

        public UnityEngine.Events.UnityEvent<Regulus.Remote.IBinder> BinderEnterEvent;
        public UnityEngine.Events.UnityEvent<Regulus.Remote.IBinder> BinderLeaveEvent;
    }

}
