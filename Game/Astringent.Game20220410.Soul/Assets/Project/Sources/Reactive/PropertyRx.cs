using System;

namespace Astringent.Game20220410.Soul.Sources
{
    public static partial class PropertyRx
    {
        public static IObservable<T> ChangeObservable<T>(this Regulus.Remote.Property<T> instance)
        {
            return new PropertyObservable<T>(instance);
        }


    }
}
