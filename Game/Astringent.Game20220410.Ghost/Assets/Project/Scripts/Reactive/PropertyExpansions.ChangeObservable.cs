using System;
using UniRx;
namespace Astringent.Game20220410
{
    public static partial class PropertyExpansions
    {
        public static IObservable<T> ChangeObservable<T>(this Regulus.Remote.Property<T> instance)
        {
            return new PropertyObservable<T>(instance);
        }


    }
}
