using UnityEngine;
using UniRx;
using System.Linq;
using Astringent.Game20220410.Protocol;
using System;
using System.Collections.Generic;

namespace Astringent.Game20220410
{
    public class ActorSpawner : AgentReactiveMonoBehaviour
    {
        
        public GameObject ActorPrefab;


        private void _Spawn(IActor obj)
        {
            UnityEngine.Debug.Log("get actor 1");
            var actor = GameObject.Instantiate(ActorPrefab).GetComponent<Actor>();
            actor.Startup(obj);
            UnityEngine.Debug.Log("get actor 2");
        }

        

        protected override IEnumerable<IDisposable> _Start(Regulus.Remote.Ghost.IAgent agent)
        {
            var addActorObs = from actor in agent.QueryNotifier<IActor>().SupplyEvent()
                              select actor;

            yield return addActorObs.Subscribe(_Spawn);
        }
    }

}
