using Regulus.Utility.WindowConsoleAppliction;
using System;

namespace Astringent.Websocket2Tcpsocket.Runner
{
    internal class Program
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="web"></param>
        /// <param name="tcp"></param>
        static void  Main(string web,string tcp)
        {
            //web = "http://*:53100/";
            //tcp = "127.0.0.1:53005";

            System.Net.IPEndPoint tcpEndPoint;
            if (!System.Net.IPEndPoint.TryParse(tcp , out tcpEndPoint))
            {
                System.Console.WriteLine("error tcp endpoint format.");
                return;
            }

            var console = new Console(tcpEndPoint);
            var webListener = new Regulus.Network.Web.Listener();
            webListener.AcceptEvent += console.Push;
            webListener.Bind(web);

            console.Run();

        }
    }
}
