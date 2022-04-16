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
        public Cinemachine.CinemachineVirtualCameraBase FollowCamera;

        private void _Spawn(IActor obj,bool is_player)
        {            
            var actor = GameObject.Instantiate(ActorPrefab).GetComponent<Actor>();
            actor.Startup(obj);            
            if(is_player)
            {
                FollowCamera.Follow = actor.transform;
                FollowCamera.LookAt = actor.transform;
            }
        }

        

        protected override IEnumerable<IDisposable> _Start(Regulus.Remote.Ghost.IAgent agent)
        {
            var addActorObs = from actor in agent.QueryNotifier<IActor>().SupplyEvent()
                              from player in agent.QueryNotifier<IPlayer>().SupplyEvent()
                              let isPlayer = player.Id.Value == actor.Id.Value
                              select new { actor , isPlayer };

            yield return addActorObs.Subscribe((ret) => { _Spawn(ret.actor, ret.isPlayer); });
        }
    }

}
