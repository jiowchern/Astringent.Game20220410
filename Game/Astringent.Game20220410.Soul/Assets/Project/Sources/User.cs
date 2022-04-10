using Astringent.Game20220410.Protocol;
using Regulus.Remote;
using System;
using Unity.Entities;
using static Astringent.Game20220410.Expansions.VectorExpansions;


namespace Astringent.Game20220410.Expansions
{
}
namespace Astringent.Game20220410.Sources
{
    public class User : System.IDisposable ,Astringent.Game20220410.Protocol.IActor , IPlayer
    {
        static int _IdProvider;
        public readonly Entity Entity;
        public readonly IBinder Binder;
        public readonly int Id;

        Property<Vector> _Vector;
        Property<Vector> _Position;

        System.Action _Remover;
        public User(Entity e, Regulus.Remote.IBinder binder)
        {
            Id = ++_IdProvider;
            Entity = e;
            this.Binder = binder;

            _Vector = new Property<Vector>();
            _Position = new Property<Vector>();
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

        Property<Vector> IActor.Vector => _Vector;

        Property<Vector> IActor.Position => _Position;

        void IDisposable.Dispose()
        {
            _Remover();
        }

        Value<int> IPlayer.SetDirection(Vector dir)
        {
            var mgr = World.DefaultGameObjectInjectionWorld.EntityManager;
            mgr.SetComponentData(Entity, new Dots.Direction() { Value = dir.ToUnity() });
            return 0;
        }
    }
}
