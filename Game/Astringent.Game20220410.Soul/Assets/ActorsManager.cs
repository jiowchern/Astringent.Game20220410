using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using Astringent.Game20220410.Protocol;
using System;

namespace Astringent.Game20220410.Scripts
{
    public class ActorsManager : MonoBehaviour
    {
        readonly System.Collections.Generic.Dictionary<IActor , Unity.Entities.Entity> _Entities;
        readonly UniRx.CompositeDisposable _Disposable;        
        ActorsManager()
        {
            _Entities = new Dictionary<IActor, Unity.Entities.Entity> ();
            _Disposable = new UniRx.CompositeDisposable ();
        }

        // Start is called before the first frame update
        void Start()
        {
            {
                var actorsObs = from agent in AgentRx.GetObservable()
                                from actor in agent.QueryNotifier<IActor>().SupplyEvent()
                                select actor;

                _Disposable.Add(actorsObs.Subscribe(_CreateActor));
            }


            {
                var actorsObs = from agent in AgentRx.GetObservable()
                                from actor in agent.QueryNotifier<IActor>().UnsupplyEvent()
                                select actor;

                _Disposable.Add(actorsObs.Subscribe(_DistroyActor));
            }

            {
                var actorsObs = from agent in AgentRx.GetObservable()
                                from actor in agent.QueryNotifier<IActor>().SupplyEvent()
                                from state in actor.MoveingState.ChangeObservable()
                                select actor ;

                _Disposable.Add(actorsObs.Subscribe(_ChnageMovingState));
            }
        }

        private void _ChnageMovingState(IActor actor)
        {
            Unity.Entities.Entity entity;
            if (_Entities.TryGetValue(actor, out entity) == false)
                return;

            Agent.GetWorld().EntityManager.SetComponentData(entity, actor.MoveingState.Value);
        }

        private void _DistroyActor(IActor actor)
        {
            var entity = _Entities[actor];
            _Entities.Remove(actor);
            if(Agent.GetWorld()!= null)
                Agent.GetWorld().EntityManager.DestroyEntity(entity);
        }

        private void _CreateActor(IActor actor)
        {

            var entiry = Agent.GetWorld().EntityManager.Instantiate(GhostPrototypesProvider.ActorEntity);
            _Entities.Add(actor,entiry);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnDisable()
        {
            _Disposable.Dispose();
        }
    }

}
