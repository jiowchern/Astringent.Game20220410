using Astringent.Game20220410.Sources;
using Regulus.Remote;
using Regulus.Remote.Server;
using Unity.Entities;

namespace Astringent.Game20220410.Dots
{
    namespace Systems
    {
        public partial class Service : Unity.Entities.SystemBase, Regulus.Remote.IEntry        
        {
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
            private readonly TcpListenSet _TcpSet;

            System.Action _Close;
            
            readonly System.Collections.Concurrent.ConcurrentDictionary<Regulus.Remote.IBinder, Sources.User> _Users;
            private readonly EntitesKeeper _Keeper;

            public Service()
            {
                _Users = new System.Collections.Concurrent.ConcurrentDictionary<IBinder, Sources.User>();

                _Keeper = new EntitesKeeper();
                var protocol = Astringent.Game20220410.Protocol.Provider.Create();
                _TcpSet = Regulus.Remote.Server.Provider.CreateTcpService(this, protocol);
                _Close = () => {
                    
                };
            }
            protected override void OnCreate()
            {
                _TcpSet.Listener.Bind(53005);

                _Close = ()=>{
                    _Users.Clear();
                    _TcpSet.Listener.Close();
                    _TcpSet.Service.Dispose();
                };

                base.OnCreate();
            }
            protected override void OnUpdate()
            {
                System.Threading.Tasks.Parallel.ForEach(_Users, (pair) => { 
                    pair.Value.Update();
                });
            }
            protected override void OnDestroy()
            {
                _Close();
                base.OnDestroy();
            }
            void IBinderProvider.AssignBinder(IBinder binder)
            {
                var user = new Sources.User(_Keeper, binder);
               
                binder.BreakEvent += () =>
                {
                    User u;
                    if(_Users.TryRemove(binder, out u))
                    {
                        u.Dispose();
                    }
                    
                };
            
                _Users.TryAdd(binder, user);
            }
        }
    }

    


}