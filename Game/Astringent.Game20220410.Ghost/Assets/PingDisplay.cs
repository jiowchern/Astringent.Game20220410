using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
namespace Astringent.Game20220410
{
    public class PingDisplay : MonoBehaviour
    {
        public UnityEngine.UI.Text Text;
        readonly UniRx.CompositeDisposable _CompositeDisposable;
        public PingDisplay()
        {
            _CompositeDisposable = new CompositeDisposable();
        }
        void Start()
        {
            var obs = from _ in UniRx.Observable.Timer(System.TimeSpan.FromSeconds(1))
                      from agent in Agent.Observer
                      from ping in agent.ObserveEveryValueChanged(a => a.Ping)
                      select ping ;

            
            _CompositeDisposable.Add(obs.Subscribe(_Display));
        }
        private void OnDestroy()
        {
            _CompositeDisposable.Clear();
        }
        // Update is called once per frame
        void Update()
        {

        }

        void _Display(float ping)
        {
            Text.text = $"{ping}";
        }
    }

}
