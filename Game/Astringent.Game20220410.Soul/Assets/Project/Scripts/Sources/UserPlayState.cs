using Astringent.Game20220410.Protocol;

using Regulus.Remote;
using Regulus.Utility;
using System;
using System.Linq;


namespace Astringent.Game20220410.Sources
{
    internal class UserPlayState : Regulus.Utility.IStatus , IPlayer
    {
        private readonly CompositeBinder _Binder;
        readonly RepeatableBinder _EntityBinder;
        private readonly Entity _Entity;
        
        private readonly EntitesKeeper _Keeper;

        public event Action DoneEvent;

        readonly System.Collections.Generic.List<int> _VisionEntites;
        readonly Property<double> _WorldTime;
        public UserPlayState(IBinder binder, EntitesKeeper keeper)
        {
            _VisionEntites = new System.Collections.Generic.List<int>();
            _EntityBinder = new RepeatableBinder(binder);
            this._Binder =new CompositeBinder( binder);            

            this._Keeper = keeper;
            _Entity = new Entity();

            _WorldTime = new Property<double>(Scripts.Service.GetWorld().Time.ElapsedTime);


        }

     

        Property<int> IPlayer.Id => new Property<int>(_Entity.Id);

        double _DeltaTime = 0;
        Property<double> IPlayer.WorldTime => _WorldTime;

        void IStatus.Enter()
        {
            //_Binder.Bind<IEntity>(_Entity);
            _Binder.Bind<IPlayer>(this);
            
            

            _Keeper.Entites.TryAdd(_Entity.Id, _Entity);
            UnityEngine.Debug.Log("enter play state");
        }

        void IStatus.Leave()
        {
            _EntityBinder.Release();
            _Binder.Unbinds();
            Entity entity;
            _Keeper.Entites.TryRemove(_Entity.Id, out entity);
            entity.Dispose();

            UnityEngine.Debug.Log("leave play state");
        }

        Value<bool> IPlayer.SetDirection(Unity.Mathematics.float3 dir   )
       {
            return _Entity.SetDirection(dir);
       }

        void IStatus.Update()
        {
            var entites = _Entity.VisionEntites;
            foreach (var entiteId in entites.Except(_VisionEntites))
            {
                Entity entity;
                if (!_Keeper.Entites.TryGetValue(entiteId, out entity))
                    continue;

                _EntityBinder.Add<IEntity>(entity);
            }

            foreach (var entiteId in _VisionEntites.Except(entites))
            {
                Entity entity;
                if (!_Keeper.Entites.TryGetValue(entiteId, out entity))
                    continue;

                _EntityBinder.Remove<IEntity>(entity);
            }
            _VisionEntites.Clear();
            _VisionEntites.AddRange(entites) ;

            _DeltaTime += Scripts.Service.GetWorld().Time.DeltaTime;
            if(_DeltaTime > 5)
            {
                _DeltaTime = 0;
                _WorldTime.Value = Scripts.Service.GetWorld().Time.ElapsedTime;
            }      
        }

        Value<bool> IPlayer.Quit()
        {
            DoneEvent();
            return true;
        }
    }
}