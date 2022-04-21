using Astringent.Game20220410.Protocol;
using System;
using UnityEngine;
using System.Linq;
using UniRx;
using System.Collections.Generic;
using Regulus.Remote.Ghost;

namespace Astringent.Game20220410
{
    public class Entity : AgentReactiveMonoBehaviour
    {

        readonly UniRx.CompositeDisposable _Disposable;
        readonly UniRx.CompositeDisposable _CommandDisposable;

        System.Action _MoveAction;

        public int Id;
        
        public Entity()
        {
            _MoveAction = () => { };
            _Disposable = new UniRx.CompositeDisposable();
            _CommandDisposable = new UniRx.CompositeDisposable();   
        }

        public void Startup(IEntity actor)
        {
            Id = actor.Id.Value;
        
            _Release();

            
            _SetMove(actor);

        }

   

        private void _SetMove(IEntity actor)
        {
            var obs = from moveState in actor.MoveingState.ChangeObservable().Repeat()
                      select moveState;

            _Disposable.Add(obs.Subscribe(_Move));
        }


        private void _Move(MoveingState state)
        {
            
            this.gameObject.transform.position = state.Position;
            
            this.gameObject.transform.Translate(state.Vector * (float)(Player.WorldTime - state.StartTime));

            _MoveAction = () => {
                this.gameObject.transform.Translate(state.Vector * UnityEngine.Time.deltaTime);
            };
        }

        private void _Release()
        {
            _Disposable.Clear();
        }



        private void Update()
        {
            _MoveAction();
        }

        new private void OnDestroy()
        {
            _Release();
            base.OnDestroy();
        }

        protected override IEnumerable<IDisposable> _Start(IAgent agent)
        {
            yield break;
        }

        internal void SetDirection(Vector3 dir)
        {
            _CommandDisposable.Clear();
            _CommandDisposable.Add((from agent in Agent.Observer
                                   from entity in agent.QueryNotifier<IEntity>().SupplyEvent()
                                   where entity.Id.Value == Id
                                   select _PreMove(entity, dir)).Subscribe());
            

        }

        private bool _PreMove(IEntity entity, Vector3 dir)
        {
            var state = entity.MoveingState.Value;
            state.Vector = dir.normalized * 1 ;
            //_Move(state);
            return true;
        }
    }

}
