using Astringent.Game20220410.Protocol;
using System;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Astringent.Game20220410.Scripts
{
    public class Actor : MonoBehaviour
    {

        readonly UniRx.CompositeDisposable _Disposable;

        System.Action _MoveAction;
        public Actor()
        {
            _MoveAction = () => { };
            _Disposable = new UniRx.CompositeDisposable();
        }

        public void SetActor(IActor actor)
        {
          //  _Release();
            //_Move(actor, actor.MoveingState.Value);

            var obs =   from moveState in actor.MoveingState.ChangeObservable()
                        select moveState ;


            
          _Disposable.Add(obs.Subscribe(_Move));

        }

        private void _Move(MoveingState state)
        {

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
    }

}
