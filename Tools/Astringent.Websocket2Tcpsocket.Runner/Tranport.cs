using Regulus.Network;
using Regulus.Network.Web;

namespace Astringent.Websocket2Tcpsocket.Runner
{
    class Tranport : System.IDisposable
    {
        
        public volatile bool Enable ;
        private readonly System.Threading.Tasks.Task _Task;

        public Tranport(Peer peer, System.Net.EndPoint tcp)
        {
            Enable = true;
            _Task = System.Threading.Tasks.Task.Run(async () =>
            {



                var tcpConnecter = new Regulus.Network.Tcp.Connecter();

                var result = await tcpConnecter.Connect(tcp);

                if (!result)
                {
                    using (peer)
                    {

                    }
                    System.Console.WriteLine("tcp connect fail.");
                    return;
                }

                peer.ErrorEvent += (e) =>
                {

                    System.Console.WriteLine($"web error {e}.");
                    Enable = false;


                };

                tcpConnecter.SocketErrorEvent += e =>
                {

                    System.Console.WriteLine($"tcp error {e}.");
                    Enable = false;


                };


                Regulus.Network.IStreamable webStream = peer;
                Regulus.Network.IStreamable tcpStream = tcpConnecter;

                

              
                using (peer)
                {
                    using (tcpConnecter)
                    {
                        var webMachine = new Regulus.Utility.StatusMachine();
                        _PushReceive(webStream, tcpStream, webMachine);

                        var tcpMachine = new Regulus.Utility.StatusMachine();
                        _PushReceive(tcpStream, webStream, tcpMachine);

                        while (Enable)
                        {
                            tcpMachine.Update();
                            webMachine.Update();
                        }
                        tcpMachine.Termination();
                        webMachine.Termination();
                        await tcpConnecter.Disconnect();
                    }
                }
              /*  var t1 = System.Threading.Tasks.Task.Run(async () =>
                {
                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    byte[] receive = new byte[16384];
                    while (Enable)
                    {

                        var receiveCount = await webStream.Receive(receive, 0, receive.Length);

                        if (!Enable)
                            break;
                        int sendCount = 0;
                        stopwatch.Restart();
                        while (receiveCount - sendCount > 0)
                        {
                            sendCount += await tcpStream.Send(receive, sendCount, receiveCount - sendCount);
                            if (!Enable)
                                break;
                        }
                        stopwatch.Stop();
                        System.Console.WriteLine($"w->t {sendCount}byte ");
                        ///System.Threading.Thread.Sleep(500);
                    }
                    await tcpConnecter.Disconnect();
                    System.Console.WriteLine($"done w->t");

                });



                var t2 = System.Threading.Tasks.Task.Run(async () =>
                {
                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    byte[] receive = new byte[16384];
                    while (Enable)
                    {

                        //System.Console.WriteLine($"done t->w 1");
                        var receiveCount = await tcpStream.Receive(receive, 0, receive.Length);
                        //System.Console.WriteLine($"done t->w 2");
                        if (!Enable)
                            break;
                        int sendCount = 0;
                        stopwatch.Restart();
                        while (receiveCount - sendCount > 0)
                        {
                            //System.Console.WriteLine($"done t->w 3");
                            sendCount += await webStream.Send(receive, sendCount, receiveCount - sendCount);
                            //System.Console.WriteLine($"done t->w 4");
                            if (!Enable)
                                break;
                        }
                        stopwatch.Stop();

                        System.Console.WriteLine($"t->w {sendCount}byte ");
                        //System.Threading.Thread.Sleep(1000);
                    }

                    System.Console.WriteLine($"done t->w");
                    System.IDisposable disposable = peer;
                    disposable.Dispose();
                });


                await t2;
                await t1;*/

            });
            
        }

        private static void _PushReceive(Regulus.Network.IStreamable receive_stream, Regulus.Network.IStreamable send_steam, Regulus.Utility.StatusMachine machine)
        {
            var receiveWeb = new Astringent.Websocket2Tcpsocket.Runner.TranportReceiveState(receive_stream);
            receiveWeb.DoneEvent += (buffer) =>
            {
                _PushSend(buffer , receive_stream,send_steam, machine);
            };
            

            machine.Push(receiveWeb);
        }

        private static void _PushSend(System.ArraySegment<byte> buffer, IStreamable reveive_stream, IStreamable send_tream, Regulus.Utility.StatusMachine machine)
        {
            var sendTcp = new Astringent.Websocket2Tcpsocket.Runner.TranportSendState(send_tream, buffer);
            sendTcp.DoneEvent += () =>
            {
                _PushReceive(reveive_stream, send_tream, machine);
            };
            machine.Push(sendTcp);
        }

        async void System.IDisposable.Dispose()
        {
            Enable = false;
            await _Task;
            System.Console.WriteLine("close.");
        }
    }
}
