using Astringent.Game20220410.Protocol;
using Regulus.Remote;
using Regulus.Utility;

namespace Astringent.Game20220410.Sources
{
    internal class UserLoginState : Regulus.Utility.IStatus , Astringent.Game20220410.Protocol.ILogin
    {
        private readonly IBinder _Binder;
        private ISoul _Soul;
        public event System.Action<string> DoneEvent;
        public UserLoginState(Regulus.Remote.IBinder binder)
        {
            this._Binder = binder;
        }

        void IStatus.Enter()
        {

            _Soul = _Binder.Bind<ILogin>(this);

            DoneEvent("123");
        }

        void IStatus.Leave()
        {
            _Binder.Unbind(_Soul);
        }

        void ILogin.Login(string username)
        {
            DoneEvent(username);
        }

        void IStatus.Update()
        {
         
        }
    }
}