using UnityEngine;
using UniRx;
using System.Linq;
using Astringent.Game20220410.Protocol;
using System;

namespace Astringent.Game20220410
{
    public class ActorSpawner : MonoBehaviour
    {
        private IDisposable _Dispose;
        public GameObject ActorPrefab;
        private void Start()
        {
            if(_Dispose !=null)
                _Dispose.Dispose();
            var addActorObs =   from agent in AgentRx.GetObservable()
                                from actor in agent.QueryNotifier<IActor>().SupplyEvent()
                                select actor;
            _Dispose = addActorObs.Subscribe(_Spawn);
        }

        private void _Spawn(IActor obj)
        {
            UnityEngine.Debug.Log("get actor 1");
            var actor = GameObject.Instantiate(ActorPrefab).GetComponent<Actor>();
            actor.Startup(obj);
            UnityEngine.Debug.Log("get actor 2");
        }

        private void OnDestroy()
        {
            _Dispose.Dispose();
        }
    }

}
