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
        private readonly Unity.Entities.Entity _AvatarEntity;
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

        public Entity()
        {
            _VisionEntites = new System.Collections.Generic.List<int>();

            UnityEngine.Debug.Log("new entity");
            Id = _IdDispenser.Dispatch(this);

            var mgr = Service.GetWorld().EntityManager;
            _Entity  = mgr.Instantiate(SoulPrototypesProvider.ActorEntity);
            mgr.SetComponentData(_Entity, new Dots.ActorAttributes { Data = new Attributes { Id = Id ,Speed = 1} });

            var linked = mgr.GetBuffer<Unity.Entities.LinkedEntityGroup>(_Entity);
            
            _VisionEntity = ( from g in linked.ToEnumerable()
                              where mgr.GetName(g.Value) == "Vision"
                      select g.Value).Single();

            _AvatarEntity = (from g in linked.ToEnumerable()
                             where mgr.GetName(g.Value) == "ActorAvatar"
                             select g.Value).Single();

            mgr.AddComponent<Unity.Transforms.Parent>(_VisionEntity);
            mgr.AddComponent<Unity.Transforms.LocalToParent>(_VisionEntity);
            mgr.SetComponentData(_VisionEntity, new Unity.Transforms.Parent { Value = _Entity });


            mgr.AddComponent<Unity.Transforms.Parent>(_AvatarEntity);
            mgr.AddComponent<Unity.Transforms.LocalToParent>(_AvatarEntity);
            mgr.SetComponentData(_AvatarEntity, new Unity.Transforms.Parent { Value = _Entity });           





            _Attributes = new Property<Attributes>();
            _MoveingState = new Property<MoveingState>();


            var eventsSystem = Service.GetWorld().GetExistingSystem<Dots.Systems.EventsSystem>();

            eventsSystem.MoveingState.StateEvent+= _Update;
            eventsSystem.Attributes.StateEvent += _Update;
            eventsSystem.TriggerEventBufferElement.StateEvent += _Update;


            UnityEngine.Debug.Log("new entity ok");
        }

        private void _Empty(int obj)
        {
            UnityEngine.Debug.Log($"empty event{obj}");
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


        private void _Update(Unity.Entities.Entity owner, Attributes arg2)
        {
            if(!owner.Equals(_Entity) )
                return;
            
            _Attributes.Value = arg2;
        }
        private void _Update(Unity.Entities.Entity owner, Dots.TriggerEventBufferElement element)
        {
            if (!owner.Equals(_VisionEntity))
                return;
            var mgr = Service.GetWorld().EntityManager;
            if(mgr.GetName(element.Entity) != "ActorAvatar")
            {
                return;
            }

            if (element.State == Dots.PhysicsEventState.Stay)
                return;

            var parent = mgr.GetComponentData<Unity.Transforms.Parent>(element.Entity);
            var attr = mgr.GetComponentData<Dots.ActorAttributes>(parent.Value);
            
            if(element.State == Dots.PhysicsEventState.Enter)
            {
                lock(_VisionEntites)
                    _VisionEntites.Add(attr.Data.Id);
            }
            else if (element.State == Dots.PhysicsEventState.Exit)
            {
                lock(_VisionEntites)
                    _VisionEntites.RemoveAll(i => i == attr.Data.Id);
            }



        }

        private void _Update(Unity.Entities.Entity owner,  MoveingState arg2)
        {
            if (!owner.Equals(_Entity))
                return;
            _MoveingState.Value = arg2;
        }

        public void Dispose()
        {
            var eventsSystem = Service.GetWorld().GetExistingSystem<Dots.Systems.EventsSystem>();

            eventsSystem.TriggerEventBufferElement.StateEvent -= _Update;
            eventsSystem.MoveingState.StateEvent -= _Update;
            eventsSystem.Attributes.StateEvent -= _Update;
            Service.GetWorld().EntityManager.DestroyEntity(_Entity);
        }
    }
}
