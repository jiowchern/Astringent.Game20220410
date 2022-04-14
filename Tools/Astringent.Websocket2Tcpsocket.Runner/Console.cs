using Regulus.Network.Web;
using System;
using System.Net;
using System.Threading;

namespace Astringent.Websocket2Tcpsocket.Runner
{
    class Console : Regulus.Utility.WindowConsole
    {
        readonly System.Collections.Generic.List<Tranport> _Tranports;
        private readonly IPEndPoint _TcpIp;
        
        public Console(System.Net.IPEndPoint tcp)
        {
            _Tranports = new System.Collections.Generic.List<Tranport>();
            this._TcpIp = tcp;
            
        }
        protected override void _Launch()
        {
            
        }

        protected override void _Shutdown()
        {
            foreach (Tranport tranport in _Tranports)
            {
                using (tranport)
                {
                    tranport.Enable = false;
                }
                
            }
        }

        protected override void _Update()
        {
            var remove = _Tranports.RemoveAll(t => t.Enable == false);            
            if(remove > 0)
            {
                System.Console.WriteLine($"remove count {remove}");
            }
        }

        internal void Push(Peer peer)
        {
            
            
          

            _Tranports.Add(new Tranport(peer , _TcpIp));
        }
    }
}
