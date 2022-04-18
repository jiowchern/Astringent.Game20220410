using Astringent.Game20220410.Protocol;
using Astringent.Game20220410.Scripts;
using Regulus.Remote;
using Regulus.Utility;
using System;
using Unity.Mathematics;

namespace Astringent.Game20220410.Sources
{
    internal class UserPlayState : Regulus.Utility.IStatus , IPlayer
    {
        private readonly DisposeBinder _Binder;
        private readonly Entity _Entity;
        
        private readonly EntitesKeeper _Keeper;

        public event Action DoneEvent;
        public UserPlayState(IBinder binder, EntitesKeeper keeper)
        {
            this._Binder =new DisposeBinder( binder);            

            this._Keeper = keeper;
            _Entity = new Entity();
        }

        Property<int> IPlayer.Id => new Property<int>(_Entity.Id);

        void IStatus.Enter()
        {
            _Binder.Bind<IEntity>(_Entity);
            _Binder.Bind<IPlayer>(this);
            
            

            _Keeper.Entites.TryAdd(_Entity.Id, _Entity);
            UnityEngine.Debug.Log("enter play state");
        }

        void IStatus.Leave()
        {
            _Binder.Unbinds();
            Entity entity;
            _Keeper.Entites.TryRemove(_Entity.Id, out entity);
            entity.Dispose();

            UnityEngine.Debug.Log("leave play state");
        }

        Value<bool> IPlayer.SetDirection(Unity.Mathematics.float3 dir)
       {
            return _Entity.SetDirection(dir);
       }

        void IStatus.Update()
        {
         
        }

        Value<bool> IPlayer.Quit()
        {
            DoneEvent();
            return true;
        }
    }
}