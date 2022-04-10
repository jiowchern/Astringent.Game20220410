using Astringent.Game20220410.Protocol;
using System.Threading.Tasks;
using UnityEngine;



namespace Astringent.Game20220410.Scripts
{
    public class Agent : MonoBehaviour
    {
        private Regulus.Remote.Client.TcpConnectSet _Set;

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
    }

}
