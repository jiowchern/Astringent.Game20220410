using System;
using System.Net;
using System.Threading.Tasks;
using Regulus.Network;
using Regulus.Network.Web;
using Regulus.Remote;

namespace Astringent.Game20220410.Soul.Sources.Web
{
    public class Listener3 : Regulus.Remote.Soul.IListenable
    {

        readonly Listener2 _Listener;
        readonly Regulus.Remote.NotifiableCollection<IStreamable> _NotifiableCollection;
        public Listener3()
        {

            _NotifiableCollection = new NotifiableCollection<IStreamable>();
            _Listener = new Listener2();

            _Listener.AcceptEvent += _Join;

        }

        event Action<IStreamable> Regulus.Remote.Soul.IListenable.StreamableEnterEvent
        {
            add
            {
                _NotifiableCollection.Notifier.Supply += value;
            }

            remove
            {
                _NotifiableCollection.Notifier.Supply -= value;
            }
        }

        event Action<IStreamable> Regulus.Remote.Soul.IListenable.StreamableLeaveEvent
        {
            add
            {
                _NotifiableCollection.Notifier.Unsupply += value;
            }

            remove
            {
                _NotifiableCollection.Notifier.Unsupply -= value;
            }
        }

        public void Bind(string address)
        {
            _Listener.Bind(address);
        }

        public void Close()
        {
            _Listener.Close();
        }

        private void _Join(Peer peer)
        {
            peer.ErrorEvent += (status) => {
                lock (_NotifiableCollection)
                    _NotifiableCollection.Items.Remove(peer);
            };
            lock (_NotifiableCollection)
                _NotifiableCollection.Items.Add(peer);
        }
    }
    public class Listener2
    {
        readonly System.Net.HttpListener _HttpListener;
        private readonly System.Threading.CancellationTokenSource _CancelGetContext;

        public Listener2()
        {
            _HttpListener = new System.Net.HttpListener();
            _CancelGetContext = new System.Threading.CancellationTokenSource();


        }
        event Action<Peer> _AcceptEvent;
        public event Action<Peer> AcceptEvent
        {
            add
            {
                _AcceptEvent += value;
            }

            remove
            {
                _AcceptEvent -= value;
            }
        }

        public void Bind(string address)
        {
            _HttpListener.Prefixes.Add(address);
            _HttpListener.Start();
            _ = _HttpListener.GetContextAsync().ContinueWith(_Listen, _CancelGetContext.Token);
        }

        private void _Listen(Task<HttpListenerContext> task)
        {
            _ = _HttpListener.GetContextAsync().ContinueWith(_Listen, _CancelGetContext.Token);
            if (task.IsCanceled)
                return;
            if (!task.IsCompleted)
            {
                return;
            }

            HttpListenerContext context = task.Result;
            if (context.Request.IsWebSocketRequest)
            {
                _Accept(context);
            }
        
        }

        private async void _Accept(HttpListenerContext context)
        {

            System.Net.WebSockets.HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);

            var webSocket = webSocketContext.WebSocket;


            var status = await Task<System.Net.WebSockets.WebSocketState>.Factory.StartNew(() =>
            {
                while (webSocket.State == System.Net.WebSockets.WebSocketState.Connecting)
                {
                    System.Threading.Thread.Sleep(1);
                }
                return webSocket.State;
            });
            if (status == System.Net.WebSockets.WebSocketState.Open)
                _AcceptEvent(new Peer(webSocket));
        }

        public void Close()
        {
            _CancelGetContext.Cancel();
            _HttpListener.Close();

        }
    }
}
