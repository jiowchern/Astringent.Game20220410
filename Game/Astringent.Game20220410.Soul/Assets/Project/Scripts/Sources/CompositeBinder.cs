using Regulus.Remote;
using System;

namespace Astringent.Game20220410.Sources
{
    class RepeatableBinder
    {
        private readonly IBinder _Binder;
        class BindPair
        {
            public ISoul Token;
            public object instance;
            public System.Type Type;
        }

        readonly System.Collections.Generic.List<BindPair> _Pairs;

        public RepeatableBinder(IBinder binder)
        {
            _Pairs = new System.Collections.Generic.List<BindPair>();
            this._Binder = binder;

        }

        public void Add<T>( T soul) where T : class
        {
            var token = _Binder.Bind<T>(soul);
            _Pairs.Add(new BindPair() { instance = soul, Token = token , Type =typeof(T) });
        }

        public void Remove<T>(T soul) where T : class
        {
            var removes = new System.Collections.Generic.List<BindPair>();
            foreach (var pair in _Pairs)
            {
                if(pair.instance == soul && pair.Type == typeof(T))
                {
                    removes.Add(pair);
                }
            }

            foreach (var remove in removes)
            {
                _Pairs.Remove(remove);
                _Binder.Unbind(remove.Token);
                return;
            }
        }

        internal void Release()
        {
            foreach (var item in _Pairs)
            {
                _Binder.Unbind(item.Token);
            }
            _Pairs.Clear();
        }
    }
    class CompositeBinder
    {
        readonly System.Collections.Generic.List<Regulus.Remote.ISoul> _Souls;
        private readonly IBinder _Binder;

        public CompositeBinder(IBinder binder)
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