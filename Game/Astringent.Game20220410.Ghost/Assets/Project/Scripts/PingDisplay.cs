using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using System;

namespace Astringent.Game20220410
{
    public class PingDisplay : AgentReactiveMonoBehaviour
    {
        public UnityEngine.UI.Text Text;
        
        public PingDisplay()
        {
        
        }

        void _Display(float ping)
        {
            Text.text = $"{ping}";
        }

        protected override IEnumerable<IDisposable> _Start(Regulus.Remote.Ghost.IAgent agent)
        {
            var obs = from _ in UniRx.Observable.Timer(System.TimeSpan.FromSeconds(1))                      
                      from ping in agent.ObserveEveryValueChanged(a => a.Ping)
                      select ping;


            yield return obs.Subscribe(_Display);
        }
    }

}
