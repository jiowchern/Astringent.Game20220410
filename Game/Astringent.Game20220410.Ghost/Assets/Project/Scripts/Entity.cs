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
        public int Iddd;
        public Entity()
        {
            _MoveAction = () => { };
            _Disposable = new UniRx.CompositeDisposable();
        }

        public void Startup(IEntity actor)
        {
            Id = actor.Id.Value;
            Iddd = Id;
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
    }

}
