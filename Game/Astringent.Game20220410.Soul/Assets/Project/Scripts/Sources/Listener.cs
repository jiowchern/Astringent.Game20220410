using Regulus.Network;
using System;
namespace Astringent.Game20220410.Soul.Sources.Web
{
}
namespace Astringent.Game20220410.Soul.Sources
{
    /*
    
     */
    class Listener : Regulus.Remote.Soul.IListenable
    {

        readonly System.Collections.Generic.List<Regulus.Remote.Soul.IListenable> _Listenables;
        public Listener()
        {
            _Listenables = new System.Collections.Generic.List<Regulus.Remote.Soul.IListenable>();
        }
        public void Add(Regulus.Remote.Soul.IListenable listenable)
        {
            _Listenables.Add(listenable);
        }
        event Action<IStreamable> Regulus.Remote.Soul.IListenable.StreamableEnterEvent
        {
            add
            {
                foreach (var listener in _Listenables)
                {
                    listener.StreamableEnterEvent += value;
                }
            }

            remove
            {
                foreach (var listener in _Listenables)
                {
                    listener.StreamableEnterEvent -= value;
                }
            }
        }

        event Action<IStreamable> Regulus.Remote.Soul.IListenable.StreamableLeaveEvent
        {
            add
            {
                foreach (var listener in _Listenables)
                {
                    listener.StreamableLeaveEvent += value;
                }
            }

            remove
            {
                foreach (var listener in _Listenables)
                {
                    listener.StreamableLeaveEvent -= value;
                }
            }
        }
    }
}
