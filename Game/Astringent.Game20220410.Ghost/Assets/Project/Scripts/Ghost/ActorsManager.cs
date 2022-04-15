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
        public GhostPrototypesProvider _Prototypes;
        readonly System.Collections.Generic.Dictionary<IActor , Actor> _Actors;
        readonly UniRx.CompositeDisposable _Disposable;        
        ActorsManager()
        {
            _Actors = new Dictionary<IActor, Actor>();
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

                _Disposable.Add(actorsObs.Subscribe(_DestroyActor));
            }
            
        }

        

        private void _DestroyActor(IActor actor)
        {
            var instance = _Actors[actor];
            _Actors.Remove(actor);
            GameObject.Destroy(instance);
        }

        private void _CreateActor(IActor actor)
        {

            var instance = GameObject.Instantiate(_Prototypes.ActorPrefab).GetComponent<Actor>();
            instance.SetActor(actor);
            _Actors.Add(actor, instance);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnDisable()
        {
            _Disposable.Clear();
        }
    }

}
