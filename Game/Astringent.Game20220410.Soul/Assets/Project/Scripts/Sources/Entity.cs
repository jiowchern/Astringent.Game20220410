using Astringent.Game20220410.Protocol;
using Astringent.Game20220410.Scripts;
using Regulus.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;

namespace Astringent.Game20220410.Sources
{
    public class Entity : IEntity , IDisposable
    {
        static IdDispenser _IdDispenser = new IdDispenser();
        private readonly Unity.Entities.Entity _VisionEntity;

        public readonly int Id;
        private readonly Unity.Entities.Entity _Entity;
        private readonly Property<Attributes> _Attributes;
        private readonly Property<MoveingState> _MoveingState;

        
        readonly System.Collections.Generic.List<int> _VisionEntites; 
        

        public System.Collections.Generic.IEnumerable<int> VisionEntites => _GetVisionEntites();

        private IEnumerable<int> _GetVisionEntites()
        {
            IEnumerable<int> array = null;
            lock (_VisionEntites)
                array = _VisionEntites.ToArray();

            return array;
        }

        public Entity(Unity.Entities.Entity e, APPEARANCE appearance)
        {
            _VisionEntites = new System.Collections.Generic.List<int>();

            _Attributes = new Property<Attributes>();
            _MoveingState = new Property<MoveingState>();

            UnityEngine.Debug.Log("new entity");
            Id = _IdDispenser.Dispatch(this);

            var mgr = Dots.Systems.Service.GetWorld().EntityManager;
            _Entity = e;
            mgr.AddComponent<Dots.MoveingState>(_Entity);
            mgr.AddComponent<Dots.Direction>(_Entity);
            mgr.AddComponent<Dots.Attributes>(_Entity);
            mgr.AddComponent<Dots.Past>(_Entity);
            mgr.AddBuffer<Dots.TriggerEventBufferElement>(_Entity);

            mgr.SetComponentData(_Entity, new Dots.Attributes { Id = Id, Data = new Attributes { Appertance = appearance } });
            mgr.SetComponentData(_Entity, new Dots.MoveingState { Speed = 1 });


            var eventsSystem = Dots.Systems.Service.GetWorld().GetExistingSystem<Dots.Systems.EventsSystem>();

            eventsSystem.MoveingState.StateEvent += _Update;
            eventsSystem.Attributes.StateEvent += _Update;
            eventsSystem.TriggerEventBufferElement.StateEvent += _UpdateVision;
            eventsSystem.TriggerEventBufferElement.StateEvent += _UpdateMoveStop;


            _VisionEntity = _CreateVision(mgr,_Entity);

            UnityEngine.Debug.Log("new entity ok");
        }

       
        private Unity.Entities.Entity _CreateVision(Unity.Entities.EntityManager mgr, Unity.Entities.Entity owner)
        {
            if(!mgr.HasComponent<Unity.Entities.LinkedEntityGroup>(owner))
                return default(Unity.Entities.Entity);

            var linked = mgr.GetBuffer<Unity.Entities.LinkedEntityGroup>(owner);
            var visisons = from g in linked.ToEnumerable()
                           where mgr.GetName(g.Value) == "Vision"
                           select g.Value;

            if (!visisons.Any())
            {
                return default(Unity.Entities.Entity);
            }
            var vision = visisons.Single();
            mgr.AddBuffer<Dots.TriggerEventBufferElement>(vision);
            mgr.AddComponent<Unity.Transforms.Parent>(vision);
            mgr.AddComponent<Unity.Transforms.LocalToParent>(vision);
            mgr.SetComponentData(vision, new Unity.Transforms.Parent { Value = owner });
            return vision;

            
        }

        internal Value<bool> SetDirection(float3 dir)
        {
            var direction = new Dots.Direction() { Value = Unity.Mathematics.math.normalizesafe(new Unity.Mathematics.float3(dir.x, 0, dir.z)) };
            Value<bool> value = new Value<bool>();
            UniRx.MainThreadDispatcher.Post((state) => {
                var mgr = Dots.Systems.Service.GetWorld().EntityManager;
                mgr.SetComponentData(_Entity, direction);
                value.SetValue(true);
                UnityEngine.Debug.Log("seeet dir");
            }, null);

            return value;
        }

        Property<int> IEntity.Id => new Regulus.Remote.Property<int>(Id);

        Property<MoveingState> IEntity.MoveingState => _MoveingState;

        Property<Attributes> IEntity.Attributes => _Attributes;


        private void _Update(Unity.Entities.Entity owner, Attributes arg2)
        {
            if(!owner.Equals(_Entity) )
                return;
            UnityEngine.Debug.Log("update =atrt");
            _Attributes.Value = arg2;
        }
        private void _UpdateMoveStop(Unity.Entities.Entity owner, Dots.TriggerEventBufferElement element)
        {
            if (element.State != Dots.PhysicsEventState.Enter)
                return;

            UnityEngine.Debug.Log("_UpdateMoveStop 1");

            if (!owner.Equals(_Entity))
                return;

            UnityEngine.Debug.Log("_UpdateMoveStop 2");

            var mgr = Dots.Systems.Service.GetWorld().EntityManager;
            if (!mgr.HasComponent<Dots.Attributes>(element.Entity))
                return;

            UnityEngine.Debug.Log("_UpdateMoveStop 3");
            var attr = mgr.GetComponentData<Dots.Attributes>(element.Entity);

            if(attr.Data.Appertance != APPEARANCE.Barrier)
                return;

            UnityEngine.Debug.Log("_UpdateMoveStop 4");
            mgr.SetComponentData(_Entity , new Dots.Direction() { Value = float3.zero});
        }

        private void _UpdateVision(Unity.Entities.Entity owner, Dots.TriggerEventBufferElement element)
        {
            if (element.State == Dots.PhysicsEventState.Stay)
                return;
            if (!owner.Equals(_VisionEntity))
                return;
            var mgr = Dots.Systems.Service.GetWorld().EntityManager;
            
            if (!mgr.HasComponent<Dots.Attributes>(element.Entity))
                return;

            var attr = mgr.GetComponentData<Dots.Attributes>(element.Entity);
            
            if(element.State == Dots.PhysicsEventState.Enter)
            {
                UnityEngine.Debug.Log("Dots.PhysicsEventState.Enter");
                lock(_VisionEntites)
                    _VisionEntites.Add(attr.Id);
            }
            else if (element.State == Dots.PhysicsEventState.Exit)
            {
                UnityEngine.Debug.Log("Dots.PhysicsEventState.Exit");
                lock (_VisionEntites)
                    _VisionEntites.RemoveAll(i => i == attr.Id);
            }



        }

        private void _Update(Unity.Entities.Entity owner,  MoveingState arg2)
        {
            if (!owner.Equals(_Entity))
                return;
            UnityEngine.Debug.Log("update =move");
            _MoveingState.Value = arg2;
        }

        public void Dispose()
        {
            var eventsSystem = Dots.Systems.Service.GetWorld().GetExistingSystem<Dots.Systems.EventsSystem>();


            eventsSystem.TriggerEventBufferElement.StateEvent -= _UpdateMoveStop;
            eventsSystem.TriggerEventBufferElement.StateEvent -= _UpdateVision;
            eventsSystem.MoveingState.StateEvent -= _Update;
            eventsSystem.Attributes.StateEvent -= _Update;

            Dots.Systems.Service.GetWorld().EntityManager.DestroyEntity(_Entity);
        }
    }
}
