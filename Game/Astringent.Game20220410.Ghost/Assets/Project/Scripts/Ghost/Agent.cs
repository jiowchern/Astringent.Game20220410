using Astringent.Game20220410.Protocol;

using UnityEngine;
using UniRx;


using Regulus.Remote.Ghost;

namespace Astringent.Game20220410.Scripts
{
    public class Agent : MonoBehaviour
    {
      
        public static IAgent FindAgent()
        {
            return  UnityEngine.GameObject.FindObjectOfType<Agent>()._Agent;
        }

        UniRx.CompositeDisposable _Disposable;

        readonly Connecter _Connecter;
        readonly string _EndPoint;
        private IAgent _Agent;

        public Agent()
        {
            
            
#if UNITY_WEBGL
            _Connecter = new WebSocketConnecter();
            _EndPoint = "127.0.0.1:53100";
#else
            _Connecter = new TcpSocketConnecter();
            _EndPoint = "127.0.0.1:53005";
#endif
            _Disposable = new CompositeDisposable();
        }
        private void Start()
        {


            _Agent = Regulus.Remote.Client.Provider.CreateAgent(Astringent.Game20220410.Protocol.Provider.Create(), _Connecter);
            
            _Agent.QueryNotifier<Astringent.Game20220410.Protocol.IPlayer>().Supply += _GetPlayer;
            /*var moveingStateObs = from plr in _Agent.QueryNotifier<Astringent.Game20220410.Protocol.IPlayer>().SupplyEvent()
                        from actor in _Agent.QueryNotifier<Astringent.Game20220410.Protocol.IActor>().SupplyEvent()
                            where actor.Id == plr.Id
                        from state in actor.MoveingState.ChangeObservable()
                        select state;

            _Disposable.Add( moveingStateObs.Subscribe(_MoveingState));*/
        }

        private void _MoveingState(MoveingState state)
        {
            UnityEngine.Debug.Log($"MoveingState {state.Vector}");
        }
        public void Disconnect()
        {
            _Connecter.Disconnect();

        }
        public void StartConnect()
        {
            _Connecter.Connect(_EndPoint);

        }
        private void _GetPlayer(IPlayer obj)
        {
            UnityEngine.Debug.Log("client get player");
        }

        private void Update()
        {
            _Connecter.Update();
            _Agent.Update();
        }

      


   

        private void OnDestroy()
        {
            _Agent.Dispose();
            _Disposable.Dispose();
        }
    }

}
