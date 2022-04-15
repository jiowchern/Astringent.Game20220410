
using Regulus.Network;
using Regulus.Remote;
using Regulus.Utility;
namespace Astringent.Game20220410
{
    public class TcpSocketConnecter : Connecter  
    {
        readonly Regulus.Network.Tcp.Connecter _Connecter;
        readonly Regulus.Network.IStreamable _Stream;
        public TcpSocketConnecter()
        {
            _Connecter = new Regulus.Network.Tcp.Connecter();
            _Stream = _Connecter;
        }
        public override async void Connect(string address)
        {
            var result = System.Text.RegularExpressions.Regex.Match(address , "(\\d+\\.\\d+\\.\\d+\\.\\d+):(\\d+)");
            if (!result.Success)
                return ;
            var ip = result.Groups[1].Value;
            var port = int.Parse(result.Groups[2].Value);
            await _Connecter.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Parse(ip), port));
        }

        public override void Disconnect()
        {
            _Connecter.Disconnect();
        }

        protected override Regulus.Remote.IWaitableValue<int> _Receive(byte[] buffer, int offset, int count)
        {
            
            return _Stream.Receive(buffer,offset,count);
        }

        protected override Regulus.Remote.IWaitableValue<int> _Send(byte[] buffer, int offset, int count)
        {
            return _Stream.Send(buffer, offset, count);
        }
    }
}
