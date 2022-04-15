using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using Astringent.Game20220410.Protocol;
using static Astringent.Game20220410.INotifierExpansions;
using static Astringent.Game20220410.PropertyExpansions;

namespace Astringent.Game20220410.Scripts
{

    public class Player : MonoBehaviour
    {
        readonly UniRx.CompositeDisposable _Disposable;
        public Player()
        {
            _Disposable = new UniRx.CompositeDisposable();
        }
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnDestroy()
        {
            _Disposable.Dispose();
        }

        public void Stop()
        {
            var obs = 
                        from agent in AgentRx.GetObservable()
                        from player in agent.QueryNotifier<IPlayer>().SupplyEvent().First()
                          from _ in player.SetDirection(new Unity.Mathematics.float3(0, 0, 0)).RemoteValue()
                          select _;


            _Disposable.Add(obs.Subscribe(i => UnityEngine.Debug.Log("stop")));
        }
        public void Move()
        {
            var obs =
                from agent in AgentRx.GetObservable()
                from player in agent.QueryNotifier<IPlayer>().SupplyEvent().First()
                      from _ in player.SetDirection(new Unity.Mathematics.float3(1, 0, 0)).RemoteValue()
                      select _;

            _Disposable.Add(obs.Subscribe(_MoveDone));
        }

        private void _MoveDone(bool obj)
        {
            UnityEngine.Debug.Log("move done");
        }
    }

}
