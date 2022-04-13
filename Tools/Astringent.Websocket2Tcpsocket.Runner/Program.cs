using System;

namespace Astringent.Websocket2Tcpsocket.Runner
{
    internal class Program
    {
        /// <summary>
    /// 
    /// </summary>
    /// <param name="webport"></param>
    /// <param name="tcpport"></param>
    
        static void  Main(ushort webport,ushort tcpport)
        {
            webport = 53100;
            tcpport = 53101;            
            
            var tcpListener = new Regulus.Network.Tcp.Listener();
            tcpListener.Bind(tcpport);
            tcpListener.AcceptEvent += (p) => 
            {

            };
            
            // create tcp connecter
            var tcpConnecter = new Regulus.Network.Tcp.Connecter();
            tcpConnecter.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1") , tcpport)).GetAwaiter().GetResult();
            
            // create web listener 
            var webListener = new Regulus.Network.Web.Listener();
            webListener.AcceptEvent += (s) => { };
            webListener.Bind($"http://*:{webport}/");
            // create web connecter
            var webConnecter = new Regulus.Network.Web.Connecter(new System.Net.WebSockets.ClientWebSocket());
            webConnecter.ConnectAsync($"ws://127.0.0.1:{webport}/").GetAwaiter().GetResult();
        }
    }
}
