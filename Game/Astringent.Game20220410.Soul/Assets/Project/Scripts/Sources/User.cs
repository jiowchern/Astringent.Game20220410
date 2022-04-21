using Astringent.Game20220410.Protocol;
using Regulus.Remote;
using System;
using Unity.Entities;



namespace Astringent.Game20220410.Sources
{
    public class User : System.IDisposable 
    {
        
        readonly EntitiesKeeper _Keeper;
        public readonly IBinder Binder;


        System.Action _Remover;

        readonly Regulus.Utility.StatusMachine _Machine;
        public User(EntitiesKeeper keeper, Regulus.Remote.IBinder binder)
        {
            _Machine = new Regulus.Utility.StatusMachine();
            
            _Keeper = keeper;
            this.Binder = binder;

            

            _Remover = () => {
               
            };

            
            


            _ToLogin();
        }

        private void _ToLogin()
        {
            var state = new UserLoginState(Binder);
            state.DoneEvent += _ToPlay;
            _Machine.Push(state);
        }

        private void _ToPlay(string name)
        {
            
            var state = new UserPlayState(Binder, _Keeper);
            state.DoneEvent += _ToLogin;
            _Machine.Push(state);
        }

        public void Update()
        {
            _Machine.Update();
        }
        

        public void Dispose()
        {
            _Machine.Termination();

            
            
            _Remover();
        }

      
    }
}
