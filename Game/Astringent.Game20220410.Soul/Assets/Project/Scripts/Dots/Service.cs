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

            public enum BINDER_COMMOND
            {
                Add,Remove
            }

            public struct BinderCommand
            {
                public BINDER_COMMOND Command;
                public Regulus.Remote.IBinder Binder;


            }
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
            readonly System.Collections.Concurrent.ConcurrentQueue<BinderCommand> _Commands;
            readonly System.Collections.Concurrent.ConcurrentDictionary<Regulus.Remote.IBinder, Sources.User> _Users;
            private readonly EntitiesKeeper _Keeper;

            public Service()
            {
                _Commands = new System.Collections.Concurrent.ConcurrentQueue<BinderCommand>();
                _Users = new System.Collections.Concurrent.ConcurrentDictionary<IBinder, Sources.User>();

                _Keeper = new EntitiesKeeper();
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

                foreach (var pair in _Users)
                {
                    pair.Value.Update();
                }               


                BinderCommand command;
                while (_Commands.TryDequeue(out command))
                {
                    if(command.Command == BINDER_COMMOND.Add)
                    {
                        var user = new Sources.User(_Keeper, command.Binder);
                        _Users.TryAdd(command.Binder, user);
                    }
                    else
                    {
                        User u;
                        if (_Users.TryRemove(command.Binder, out u))
                        {
                            u.Dispose();
                        }

                    }
                }
            }
            protected override void OnDestroy()
            {
                _Close();
                base.OnDestroy();
            }
            void IBinderProvider.AssignBinder(IBinder binder)
            {
               
                binder.BreakEvent += () =>
                {
                    _Commands.Enqueue(new BinderCommand() { Command = BINDER_COMMOND.Remove, Binder = binder });                   
                };

                _Commands.Enqueue(new BinderCommand() { Command = BINDER_COMMOND.Add, Binder = binder });

            }
        }
    }

    


}