using Astringent.Game20220410.Protocol;
using Astringent.Game20220410.Scripts;
using Regulus.Remote;
using System;
using Unity.Mathematics;

namespace Astringent.Game20220410.Sources
{
    public class Entity : IEntity , IDisposable
    {
        static IdDispenser _IdDispenser = new IdDispenser();
        public readonly int Id;
        private readonly Unity.Entities.Entity _Entity;
        private readonly Property<Attributes> _Attributes;
        private readonly Property<MoveingState> _MoveingState;
        
        public Entity()
        {
            UnityEngine.Debug.Log("new entity");
            Id = _IdDispenser.Dispatch(this);

            var mgr = Service.GetWorld().EntityManager;
            _Entity  = mgr.Instantiate(SoulPrototypesProvider.ActorEntity);
            mgr.SetComponentData(_Entity, new Dots.ActorAttributes { Data = new Attributes { Id = Id ,Speed = 1} });

            _Attributes = new Property<Attributes>();
            _MoveingState = new Property<MoveingState>();


            var eventsSystem = Service.GetWorld().GetExistingSystem<Dots.Systems.EventsSystem>();

            eventsSystem.MoveingState.StateEvent+= _Update;
            eventsSystem.Attributes.StateEvent += _Update;


            UnityEngine.Debug.Log("new entity ok");
        }

        internal Value<bool> SetDirection(float3 dir)
        {
            var direction = new Dots.Direction() { Value = Unity.Mathematics.math.normalizesafe(new Unity.Mathematics.float3(dir.x, 0, dir.z)) };
            Value<bool> value = new Value<bool>();
            UniRx.MainThreadDispatcher.Post((state) => {
                var mgr = Scripts.Service.GetWorld().EntityManager;
                mgr.SetComponentData(_Entity, direction);
                value.SetValue(true);
            }, null);

            return value;
        }

        Property<int> IEntity.Id => new Regulus.Remote.Property<int>(Id);

        Property<MoveingState> IEntity.MoveingState => _MoveingState;

        Property<Attributes> IEntity.Attributes => _Attributes;


        private void _Update(int id, Attributes arg2)
        {
            if (id != Id)
                return;
            _Attributes.Value = arg2;
        }
        private void _Update(int id, MoveingState arg2)
        {
            if (id != Id)
                return;
            _MoveingState.Value = arg2;
        }

        public void Dispose()
        {
            var eventsSystem = Service.GetWorld().GetExistingSystem<Dots.Systems.EventsSystem>();

            eventsSystem.MoveingState.StateEvent -= _Update;
            eventsSystem.Attributes.StateEvent -= _Update;
            Service.GetWorld().EntityManager.DestroyEntity(_Entity);
        }
    }
}
