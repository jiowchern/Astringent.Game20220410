using Astringent.Game20220410.Protocol;
using Regulus.Remote;
using System;
using Unity.Entities;



namespace Astringent.Game20220410.Sources
{
    public class User : System.IDisposable 
    {
        
        public readonly EntitesKeeper _Keeper;
        public readonly IBinder Binder;
        

        
        System.Action _Remover;

        readonly Regulus.Utility.StatusMachine _Machine;
        public User(EntitesKeeper keeper, Regulus.Remote.IBinder binder)
        {
            _Machine = new Regulus.Utility.StatusMachine();
            
            _Keeper = keeper;
            this.Binder = binder;

            

            _Remover = () => {
               
            };

            UnityEngine.Debug.Log("server get player");
            


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
            UnityEngine.Debug.Log("play state");
            var state = new UserPlayState(Binder, _Keeper);
            state.DoneEvent += _ToLogin;
            _Machine.Push(state);
        }

        public void Update()
        {
            _Machine.Update();
        }
        

        void IDisposable.Dispose()
        {
            _Machine.Termination();

            
            
            _Remover();
        }

      
    }
}
