using Astringent.Game20220410.Protocol;
using Regulus.Remote;
using System;
using Unity.Entities;



namespace Astringent.Game20220410.Sources
{
    public class User : System.IDisposable ,Astringent.Game20220410.Protocol.IActor , IPlayer
    {
        static int _IdProvider;
        public readonly Entity Entity;
        public readonly IBinder Binder;
        public readonly int Id;

        Property<MoveingState> _MoveingState;
        Property<ActorAttributes> _Attributes;
        float _SyncMovingStateInterval;
        System.Action _Remover;
        public User(Entity entity, Regulus.Remote.IBinder binder)
        {
            Id = ++_IdProvider;
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
        }

        Property<int> IActor.Id => new Property<int>(Id);

        Property<int> IPlayer.Id => new Property<int>(Id);

        Property<MoveingState> IActor.MoveingState => _MoveingState;

        Property<ActorAttributes> IActor.Attributes => _Attributes;

        void IDisposable.Dispose()
        {
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
                var moveing = mgr.GetComponentData<MoveingState>(Entity);
                if (moveing.Equals(_MoveingState.Value))
                    return;
                _MoveingState.Value = moveing;
            }

            {
                var component = mgr.GetComponentData<ActorAttributes>(Entity);
                if (component.Equals(_Attributes.Value))
                    return;
                _Attributes.Value = component;
            }

        }
        Value<int> IPlayer.SetDirection(Unity.Mathematics.float3 dir)
        {
            var direction = new Dots.Direction() { Value = Unity.Mathematics.math.normalizesafe(dir) };
            Value<int> value = new Value<int>();
            UniRx.MainThreadDispatcher.Post((state) => {
                var mgr = Scripts.Service.GetWorld().EntityManager;                
                mgr.SetComponentData(Entity, direction); 
                value.SetValue(0);
            } , null);
            
            return value;
        }
    }
}
