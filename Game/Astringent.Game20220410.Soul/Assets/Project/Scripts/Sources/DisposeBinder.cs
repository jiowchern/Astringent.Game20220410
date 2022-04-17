using Regulus.Remote;

namespace Astringent.Game20220410.Sources
{
    class DisposeBinder
    {
        readonly System.Collections.Generic.List<Regulus.Remote.ISoul> _Souls;
        private readonly IBinder _Binder;

        public DisposeBinder(IBinder binder)
        {
            _Souls = new System.Collections.Generic.List<ISoul>();
            this._Binder = binder;
        }

        public void Bind<T>(T soul)
        {
            _Souls.Add(_Binder.Bind<T>(soul));
        }
        public void Unbinds()
        {
            foreach (var soul in _Souls)
            {
                _Binder.Unbind(soul);
            }
            _Souls.Clear();
        }
    }
}