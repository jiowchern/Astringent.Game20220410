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

        Property<Unity.Mathematics.float3> _Vector;
        Property<Unity.Mathematics.float3> _Position;
        float _SampelInterval;
        System.Action _Remover;
        public User(Entity entity, Regulus.Remote.IBinder binder)
        {
            Id = ++_IdProvider;
            Entity = entity;
            this.Binder = binder;

            _Vector = new Property<Unity.Mathematics.float3>();
            _Position = new Property<Unity.Mathematics.float3>();
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

        Property<Unity.Mathematics.float3> IActor.Vector => _Vector;

        Property<Unity.Mathematics.float3> IActor.Position => _Position;

        void IDisposable.Dispose()
        {
            _Remover();
        }

        public void StateSample()
        {
            _SampelInterval += UnityEngine.Time.deltaTime;
            if (_SampelInterval < 1f / 20f)
                return;
            _SampelInterval = 0;
            var mgr = Scripts.Service.GetWorld().EntityManager;
            var moveing = mgr.GetComponentData<Dots.MoveingState>(Entity);
            
            if(Unity.Mathematics.math.all(_Vector.Value != moveing.Vector))
                _Vector.Value = moveing.Vector;
            if (Unity.Mathematics.math.all(_Position.Value != moveing.Position))
                _Position.Value = moveing.Position;
        }
        Value<int> IPlayer.SetDirection(Unity.Mathematics.float3 dir)
        {
            Value<int> value = new Value<int>();
            UniRx.MainThreadDispatcher.Post((state) => {
                var mgr = Scripts.Service.GetWorld().EntityManager;
                mgr.SetComponentData(Entity, new Dots.Direction() { Value = dir }); 
                value.SetValue(0);
            } , null);
            
            return value;
        }
    }
}
