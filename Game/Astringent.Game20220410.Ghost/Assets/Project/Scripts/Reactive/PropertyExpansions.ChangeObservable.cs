using System;
using System.Collections;

namespace Astringent.Game20220410
{
    public static class PropertyExpansions
    {
      
        public static IObservable<T> ChangeObservable<T>(this Regulus.Remote.Property<T> instance)
        {
            return UniRx.Observable.FromCoroutine<T>( (obs) => _RunWait(obs , instance));


            
        }

        private static IEnumerator _RunWait<T>(IObserver<T> observer , Regulus.Remote.Property<T> instance)
        {
            observer.OnNext(instance.Value);
            
            var val = instance.Value;
            while(val.Equals(instance.Value))
            {
                yield return null;
            }            
            
            observer.OnNext(instance.Value);
            observer.OnCompleted();
        }


    }
}
