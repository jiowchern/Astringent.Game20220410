using System;
using System.Collections;
using Regulus.Remote.Ghost;

namespace Astringent.Game20220410
{
    public static class AgentRx
    {
        public static IObservable<IAgent> GetObservable()
        {
            return UniRx.Observable.FromCoroutine<IAgent>(_RunWaitAgent);
        }
        private static IEnumerator _RunWaitAgent(IObserver<IAgent> observer)
        {
            
            var agent = Scripts.Agent.FindAgent();
            while (agent == null)
            {
                yield return null;
                agent = Scripts.Agent.FindAgent();
            }
            observer.OnNext(agent);
            observer.OnCompleted();
        }
    }

}
