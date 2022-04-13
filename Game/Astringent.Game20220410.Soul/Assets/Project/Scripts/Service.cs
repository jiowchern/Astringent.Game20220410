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
    public class Service : MonoBehaviour , Regulus.Remote.IEntry
    {
        public readonly static string WorldName = "server";
        
        public static World GetWorld()
        {
            /*foreach (var world in World.All)
            {
                if (world.Name != WorldName)
                    continue;
                return world;                
            }
            return new World(WorldName);*/

            return World.DefaultGameObjectInjectionWorld;
        }

        public int TcpPort;
        public int WebPort;

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
            var listener = new Soul.Sources.Listener();
            {
                var tcp = new Regulus.Remote.Server.Tcp.Listener();
                listener.Add(tcp);
                tcp.Bind(TcpPort);
                _Closes.Add(() => tcp.Close());
            }

            
            {
                var web = new Soul.Sources.Web.Listener3();
                Regulus.Remote.Soul.IListenable listenable = web;
                listenable.StreamableEnterEvent += (s) =>
                {
                    UnityEngine.Debug.Log("web connected");
                };
                listener.Add(web);
                web.Bind($"http://*:{WebPort}/");
                _Closes.Add(() => web.Close());
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
