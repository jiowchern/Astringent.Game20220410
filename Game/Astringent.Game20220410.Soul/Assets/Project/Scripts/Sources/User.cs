using Astringent.Game20220410.Protocol;
using Regulus.Remote;
using System;
using Unity.Entities;



namespace Astringent.Game20220410.Sources
{

    public class User : System.IDisposable ,Astringent.Game20220410.Protocol.IActor , IPlayer
    {
        
        public readonly Entity Entity;
        public readonly IBinder Binder;
        public readonly long Id;

        Property<MoveingState> _MoveingState;
        Property<ActorAttributes> _Attributes;
        float _SyncMovingStateInterval;
        System.Action _Remover;
        public User(long id,Entity entity, Regulus.Remote.IBinder binder)
        {
            Id = id;
            Entity = entity;
            this.Binder = binder;

            _MoveingState = new Property<MoveingState>();
            _Attributes = new Property<ActorAttributes>();
            var actor = Binder.Bind<IActor>(this);
            var player = Binder.Bind<IPlayer>(this);

            _Remover = () => {
                Binder.Unbind(actor);
                Binder.Unbind(player);
            };

            UnityEngine.Debug.Log("server get player");
            Scripts.Service.GetWorld().GetExistingSystem<Dots.Systems.MoveSystem>().StateEvent += _UpdateMoveingState;
        }

        private void _UpdateMoveingState(long id, MoveingState obj)
        {
            if (id != Id)
                return;

            _MoveingState.Value = obj;
        }

        Property<long> IActor.Id => new Property<long>(Id);

        Property<long> IPlayer.Id => new Property<long>(Id);

        Property<MoveingState> IActor.MoveingState => _MoveingState;

        Property<ActorAttributes> IActor.Attributes => _Attributes;

        void IDisposable.Dispose()
        {
            Scripts.Service.GetWorld().GetExistingSystem<Dots.Systems.MoveSystem>().StateEvent -= _UpdateMoveingState;
            Scripts.Service.GetWorld().EntityManager.DestroyEntity(Entity);
            _Remover();
        }

        public void SyncStates()
        {
            _SyncMovingStateInterval += UnityEngine.Time.deltaTime;
            if (_SyncMovingStateInterval < 1f / 20f)
                return;
            _SyncMovingStateInterval = 0;
            var mgr = Scripts.Service.GetWorld().EntityManager;

          

            {
                
                var component = mgr.GetComponentData<Dots.ActorAttributes>(Entity);
                if (component.Data.Equals(_Attributes.Value))
                    return;
                _Attributes.Value = component.Data;
            }

        }
        Value<bool> IPlayer.SetDirection(Unity.Mathematics.float3 dir)
        {
            var direction = new Dots.Direction() { Value = Unity.Mathematics.math.normalizesafe(new Unity.Mathematics.float3(dir.x,0,dir.z)) };
            Value<bool> value = new Value<bool>();
            UniRx.MainThreadDispatcher.Post((state) => {
                var mgr = Scripts.Service.GetWorld().EntityManager;                
                mgr.SetComponentData(Entity, direction); 
                value.SetValue(true);
            } , null);
            
            return value;
        }
    }
}
