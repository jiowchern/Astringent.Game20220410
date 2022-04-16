using UnityEngine;
using UniRx;
using System;
using System.Collections.Generic;

namespace Astringent.Game20220410
{
    public abstract class AgentReactiveMonoBehaviour : MonoBehaviour
    {
        readonly UniRx.CompositeDisposable _CompositeDisposable;
        System.IDisposable _Disposable;
        public AgentReactiveMonoBehaviour()
        {
            _CompositeDisposable = new CompositeDisposable();
        }

        public System.IObservable<Regulus.Remote.Ghost.IAgent> Observer => Agent.Observer;

        protected void OnDestroy()
        {
            _CompositeDisposable.Clear();
            _Disposable.Dispose();
        }


        protected void Start()
        {
            var obs = from a in Agent.Observer
                      select a;
            _Disposable = obs.Subscribe(_Setup);
        }

        private void _Setup(Regulus.Remote.Ghost.IAgent agent)
        {
            _CompositeDisposable.Clear();
            System.Collections.Generic.IEnumerable<System.IDisposable> observers = _Start(agent);
            foreach (var observer in observers)
            {
                _CompositeDisposable.Add(observer);
            }
        }

        protected abstract IEnumerable<IDisposable> _Start(Regulus.Remote.Ghost.IAgent agent);
        
    }

}
