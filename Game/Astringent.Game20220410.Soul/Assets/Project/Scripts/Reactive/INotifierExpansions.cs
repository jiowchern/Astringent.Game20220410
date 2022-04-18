using Regulus.Remote;
using System;
using UniRx;

namespace Astringent.Game20220410
{
    public static class DynamicBufferExpansions
    {
        public static System.Collections.Generic.IEnumerable<T> ToEnumerable<T>(this Unity.Entities.DynamicBuffer<T> buffer) where T : unmanaged
        {
            var length = buffer.Length;
            for (int i = 0; i < length; i++)
            {
                yield return buffer[i];
            }
        }
    }
    public static class INotifierExpansions
    {
        //DynamicBuffer<T>

        public static IObservable<TValue> RemoteValue<TValue>(this Regulus.Remote.Value<TValue> ret)
        {
            return new OnceRemoteReturnValueEvent<TValue>(ret);
        }

        public static IObservable<TGpi> SupplyEvent<TGpi>(this INotifier<TGpi> queryable)
        {
            return Observable.FromEvent<Action<TGpi>, TGpi>(h => (gpi) => h(gpi), h => queryable.Supply += h, h => queryable.Supply -= h);
        }

        public static IObservable<TGpi> UnsupplyEvent<TGpi>(this INotifier<TGpi> queryable)
        {
            return Observable.FromEvent<Action<TGpi>, TGpi>(h => (gpi) => h(gpi), h => queryable.Unsupply += h, h => queryable.Unsupply -= h);
        }
        public static IObservable<TGpi> NotifierSupply<TGpi>(this INotifierQueryable queryable)
        {
            return Observable.FromEvent<Action<TGpi>, TGpi>(h => (gpi) => h(gpi), h => queryable.QueryNotifier<TGpi>().Supply += h, h => queryable.QueryNotifier<TGpi>().Supply -= h);
        }

        public static IObservable<TGpi> NotifierUnsupply<TGpi>(this INotifierQueryable queryable)
        {
            return Observable.FromEvent<Action<TGpi>, TGpi>(h => (gpi) => h(gpi), h => queryable.QueryNotifier<TGpi>().Unsupply += h, h => queryable.QueryNotifier<TGpi>().Unsupply -= h);
        }
    }
}
