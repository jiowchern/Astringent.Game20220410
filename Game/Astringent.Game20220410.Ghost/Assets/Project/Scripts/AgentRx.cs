using Regulus.Remote.Ghost;
using System.Collections;

using System;
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

            var agent = Agent.FindAgent();
            while (agent == null)
            {
                yield return null;
                agent = Agent.FindAgent();
            }
            observer.OnNext(agent);
            observer.OnCompleted();
        }
    }

}
