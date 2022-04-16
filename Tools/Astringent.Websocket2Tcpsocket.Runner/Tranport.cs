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
                
                peer.ErrorEvent += e => {
                    
                    System.Console.WriteLine($"web error {e}.");                    
                    Enable = false;

                    System.IDisposable disposable = peer;
                    disposable.Dispose();                    
                    tcpConnecter.Disconnect();
                };

                tcpConnecter.SocketErrorEvent += e => {
                    if (e == System.Net.Sockets.SocketError.Success)
                        return;
                    System.Console.WriteLine($"tcp error {e}.");
                    Enable = false;

                    System.IDisposable disposable = peer;
                    disposable.Dispose();
                };

                if (!result)
                {
                    using (peer)
                    {

                    }
                    System.Console.WriteLine("tcp connect fail.");
                    return;
                }


                Regulus.Network.IStreamable webStream = peer;
                Regulus.Network.IStreamable tcpStream = tcpConnecter;
                var t1 = System.Threading.Tasks.Task.Run(async () => {
                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    byte[] receive = new byte[16384];
                    while (Enable)
                    {
                        var receiveCount = await webStream.Receive(receive, 0, receive.Length);
                        int sendCount = 0;
                        stopwatch.Restart();
                        while (receiveCount - sendCount > 0)
                        {
                            sendCount += await tcpStream.Send(receive, sendCount, receiveCount - sendCount);
                        }
                        stopwatch.Stop();
                        System.Console.WriteLine($"w->t {sendCount}byte {stopwatch.Elapsed.TotalSeconds}s");
                    }

                    System.Console.WriteLine($"done w->t");
                    
                });



                var t2 = System.Threading.Tasks.Task.Run(async () =>
                {
                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    byte[] receive = new byte[16384];
                    while (Enable)
                    {
                        var receiveCount = await tcpStream.Receive(receive, 0, receive.Length);
                        
                        int sendCount = 0;
                        stopwatch.Restart();
                        while (receiveCount - sendCount > 0)
                        {
                            sendCount += await webStream.Send(receive, sendCount, receiveCount - sendCount);
                        }
                        stopwatch.Stop();

                        System.Console.WriteLine($"t->w {sendCount}byte {stopwatch.Elapsed.TotalSeconds}s");
                        
                    }

                    System.Console.WriteLine($"done t->w");
                    
                });
               
                await t2;
                await t1;

            });
            
        }

        async void System.IDisposable.Dispose()
        {
            Enable = false;
            await _Task;
            System.Console.WriteLine("close.");
        }
    }
}
