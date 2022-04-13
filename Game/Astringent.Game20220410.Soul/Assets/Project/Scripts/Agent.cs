using Astringent.Game20220410.Protocol;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;
using System.Linq;
using static Astringent.Game20220410.Soul.Sources.INotifierExpansions;
using static Astringent.Game20220410.Soul.Sources.PropertyExpansions;
using System;
using Regulus.Remote.Ghost;

namespace Astringent.Game20220410.Scripts
{
    public class Agent : MonoBehaviour
    {
        
        UniRx.CompositeDisposable _Disposable;

        public Connecter TcpConnecter;
        public Connecter WebConnecter;
        private IAgent _Agent;

        public Agent()
        {
            _Disposable = new CompositeDisposable();
        }
        private void Start()
        {
#if UNITY_EDITOR || !UNITY_WEBGL
            //_Agent = Regulus.Remote.Client.Provider.CreateAgent(Astringent.Game20220410.Protocol.Provider.Create(), TcpConnecter);
            _Agent = Regulus.Remote.Client.Provider.CreateAgent(Astringent.Game20220410.Protocol.Provider.Create(), WebConnecter);
#else
            
#endif


            _Agent.QueryNotifier<Astringent.Game20220410.Protocol.IPlayer>().Supply += _GetPlayer;
            var moveingStateObs = from plr in _Agent.QueryNotifier<Astringent.Game20220410.Protocol.IPlayer>().SupplyEvent()
                        from actor in _Agent.QueryNotifier<Astringent.Game20220410.Protocol.IActor>().SupplyEvent()
                            where actor.Id == plr.Id
                        from state in actor.MoveingState.ChangeObservable()
                        select state;

            _Disposable.Add( moveingStateObs.Subscribe(_MoveingState));
        }

        private void _MoveingState(MoveingState state)
        {
            UnityEngine.Debug.Log($"MoveingState {state.Vector}");
        }

        public void StartConnect()
        {
#if UNITY_EDITOR || !UNITY_WEBGL
            //TcpConnecter.Connect("127.0.0.1:53772");
            WebConnecter.Connect("127.0.0.1:53771");
#else
            
#endif
        }
        private void _GetPlayer(IPlayer obj)
        {
            UnityEngine.Debug.Log("client get player");
        }

        private void Update()
        {
            _Agent.Update();
        }

        public void Stop()
        {
            var obs = from player in _Agent.QueryNotifier<IPlayer>().SupplyEvent().First()
                      from _ in player.SetDirection(new Unity.Mathematics.float3(0, 0, 0)).RemoteValue()
                      select _;


            _Disposable.Add(obs.ObserveOnMainThread().Subscribe(i => UnityEngine.Debug.Log("stop"))); 
        }
        public void Move()
        {
            var obs = from player in _Agent.QueryNotifier<IPlayer>().SupplyEvent().First()
                      from _ in player.SetDirection(new Unity.Mathematics.float3(1, 0, 0)).RemoteValue()
                      select _;

            _Disposable.Add(obs.ObserveOnMainThread().Subscribe(_MoveDone));
        }


        private void _MoveDone(bool obj)
        {
            UnityEngine.Debug.Log("move done");
        }

        private void OnDestroy()
        {
            _Disposable.Dispose();
        }
    }

}
