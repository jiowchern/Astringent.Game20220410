using Astringent.Game20220410.Protocol;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;
using System.Linq;
using static Astringent.Game20220410.Soul.Sources.INotifierQueryableRx;
using System;

namespace Astringent.Game20220410.Scripts
{
    public class Agent : MonoBehaviour
    {
        private Regulus.Remote.Client.TcpConnectSet _Set;
        UniRx.CompositeDisposable _Disposable;

        public Agent()
        {
            _Disposable = new CompositeDisposable();
        }
        private void Start()
        {

            var set = Regulus.Remote.Client.Provider.CreateTcpAgent(Astringent.Game20220410.Protocol.Provider.Create());
            

            _Set = set;

            _Set.Agent.QueryNotifier<Astringent.Game20220410.Protocol.IPlayer>().Supply += _GetPlayer;
        }
        public void StartConnect()
        {
            _Set.Connecter.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 53003));
        }
        private void _GetPlayer(IPlayer obj)
        {
            UnityEngine.Debug.Log("client get player");
        }

        private void Update()
        {
            _Set.Agent.Update();
        }

        public void Stop()
        {
            var obs = from player in _Set.Agent.QueryNotifier<IPlayer>().SupplyEvent().First()
                      from _ in player.SetDirection(new Unity.Mathematics.float3(0, 0, 0)).RemoteValue()
                      select _;


            _Disposable.Add(obs.ObserveOnMainThread().Subscribe(i => UnityEngine.Debug.Log("stop"))); 
        }
        public void Move()
        {
            var obs = from player in _Set.Agent.QueryNotifier<IPlayer>().SupplyEvent().First()
                      from _ in _Move(player)
                      select _;


            _Disposable.Add(obs.ObserveOnMainThread().Subscribe(_MoveDone));
        }

        private static IObservable<int> _Move(IPlayer player)
        {
            return player.SetDirection(new Unity.Mathematics.float3(1, 0, 0)).RemoteValue();
        }

        private void _MoveDone(int obj)
        {
            UnityEngine.Debug.Log("move done");
        }

        private void OnDestroy()
        {
            _Disposable.Dispose();
        }
    }

}
