﻿using Astringent.Game20220410.Protocol;
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

        System.Action _MoveAction;

        public int Id { get; private set; }

        public Entity()
        {
            _MoveAction = () => { };
            _Disposable = new UniRx.CompositeDisposable();
        }

        public void Startup(IEntity actor)
        {
            Id = actor.Id.Value;
            _Release();

            _SetDestroy(actor);
            _SetMove(actor);

        }

        private void _SetDestroy(IEntity actor)
        {
            var obs = from agent in Observer
                      from a in agent.QueryNotifier<IEntity>().UnsupplyEvent()
                      where a == actor
                      select a;

            _Disposable.Add(obs.Subscribe(_Destroy));
        }

        private void _Destroy(IEntity actor)
        {
            GameObject.Destroy(gameObject);
        }

        private void _SetMove(IEntity actor)
        {
            var obs = from moveState in actor.MoveingState.ChangeObservable().Repeat()
                      select moveState;

            _Disposable.Add(obs.Subscribe(_Move));
        }

        private void _Move(MoveingState state)
        {
            UnityEngine.Debug.Log($"get actor move {state.Position}");
            this.gameObject.transform.position = state.Position;
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
    }

}