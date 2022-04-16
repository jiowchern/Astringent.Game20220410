using UnityEngine;

using System.Linq;
using UniRx;
using Astringent.Game20220410.Protocol;
using System.Collections.Generic;
using System;
using Regulus.Remote.Ghost;


namespace Astringent.Game20220410
{
    public class Player : AgentReactiveMonoBehaviour 
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
            _Disposable.Clear();
        }

        public void Stop()
        {
            _Disposable.Clear();    
            var obs =
                        from agent in Observer
                        from player in agent.QueryNotifier<IPlayer>().SupplyEvent()
                        from _ in player.SetDirection(new Unity.Mathematics.float3(0, 0, 0)).RemoteValue()
                        select _;


            _Disposable.Add(obs.First().Subscribe(i => UnityEngine.Debug.Log("stop")));
        }
        public void Move()
        {
            _Disposable.Clear();
            var obs =
                from agent in Observer
                from player in agent.QueryNotifier<IPlayer>().SupplyEvent()
                from _ in player.SetDirection(new Unity.Mathematics.float3(1, 0, 0)).RemoteValue()
                select _;

            _Disposable.Add(obs.First().Subscribe(_MoveDone));
        }

        private void _MoveDone(bool obj)
        {
            UnityEngine.Debug.Log("move done");
        }

        protected override IEnumerable<IDisposable> _Start(IAgent agent)
        {


            yield break;
        }

        private void _Follow(Actor obj)
        {
          
        }
    }

}
