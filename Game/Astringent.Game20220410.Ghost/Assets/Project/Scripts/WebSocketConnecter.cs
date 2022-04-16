using Regulus.Remote.Client.Tcp;
using System;

using NativeWebSocket;
using Regulus.Network;

namespace Astringent.Game20220410
{
    public class WebSocketConnecter : Connecter
    {

        NativeWebSocket.WebSocket _Socket;

        readonly System.Collections.Concurrent.ConcurrentQueue<byte> _Reads;
        readonly System.Collections.Concurrent.ConcurrentQueue<byte> _Writes;
        public event System.Action CloseEvent; 
        public WebSocketConnecter()
        {
            CloseEvent += () => { };
            _Socket = new WebSocket("ws://127.0.0.1:1111");
            _Reads = new System.Collections.Concurrent.ConcurrentQueue<byte>();
            _Writes = new System.Collections.Concurrent.ConcurrentQueue<byte>();


        }
        private async void Update()
        {

#if UNITY_EDITOR || !UNITY_WEBGL
            _Socket.DispatchMessageQueue();
#endif

            if(_Socket.State == WebSocketState.Open)
            {
                byte data;
                System.Collections.Generic.List<byte> buffer = new System.Collections.Generic.List<byte>();
                while(_Writes.TryDequeue(out data))
                {
                    buffer.Add(data);
                }
                if(buffer.Count > 0)
                    await _Socket.Send(buffer.ToArray());
            }
        }


        private void _Close(WebSocketCloseCode closeCode)
        {

            CloseEvent();
        }

        private void _Message(byte[] data)
        {
            
            foreach (var b in data)
            {
                _Reads.Enqueue(b);
            }
            
        }

        private void _Error(string error)
        {
            UnityEngine.Debug.Log($"error {error}");
        }

        private void _Open()
        {
            UnityEngine.Debug.Log("open");
        }

        protected override Regulus.Remote.IWaitableValue<int> _Receive(byte[] buffer, int offset, int count)
        {
            byte data;
            int i = 0;

            while (_Reads.TryDequeue(out data))
            {

                buffer[offset + i++] = data;
                if (i == count)
                    break;
            }
                     
            

            return new Regulus.Network.NoWaitValue<int>(i);
        }

        protected override Regulus.Remote.IWaitableValue<int> _Send(byte[] buffer, int offset, int count)
        {
            for (int i = offset; i < offset + count; i++)
            {
                _Writes.Enqueue(buffer[i]);
            }
       
            
            
            return new Regulus.Network.NoWaitValue<int>(count);
        }

        public override async void Connect(string address)
        {
            var result = System.Text.RegularExpressions.Regex.Match(address, "([\\w\\.]+):(\\d+)");
            if (!result.Success)
                return ;
            var ip = result.Groups[1].Value;
            var port = int.Parse(result.Groups[2].Value);


            //Disconnect();
            _Socket = new WebSocket($"ws://{ip}:{port}");
            _Socket.OnOpen += _Open;
            _Socket.OnError += _Error;
            _Socket.OnMessage += _Message;
            _Socket.OnClose += _Close;
            await _Socket.Connect();
            UnityEngine.Debug.Log($"connected");
            
        }

        public override void Disconnect()
        {
            if (_Socket.State == WebSocketState.Closed)
                return;
            _Socket.Close();
            _Socket.OnOpen -= _Open;
            _Socket.OnError -= _Error;
            _Socket.OnMessage -= _Message;
            _Socket.OnClose -= _Close;
            
        }
    }
}
