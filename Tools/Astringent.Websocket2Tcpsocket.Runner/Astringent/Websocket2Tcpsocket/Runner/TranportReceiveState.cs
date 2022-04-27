using Regulus.Network;
using Regulus.Remote;
using Regulus.Utility;
using System;

namespace Astringent.Websocket2Tcpsocket.Runner
{
    internal class TranportReceiveState : Regulus.Utility.IStatus
    {
     
        private readonly IAwaitable<int> _Waiter;
        readonly byte[] _Buffer;

        public event System.Action<System.ArraySegment<byte>> DoneEvent;
        public TranportReceiveState(IStreamable stream)
        {
            _Buffer= new byte[16384];
            
            _Waiter = stream.Receive(_Buffer, 0, _Buffer.Length).GetAwaiter();
        }

        void IStatus.Enter()
        {
            
        }

        void IStatus.Leave()
        {
            
        }

        void IStatus.Update()
        {
            if(_Waiter.IsCompleted)
            {
                var count = _Waiter.GetResult();
                Regulus.Utility.Log.Instance.WriteInfo($"receive {count}byte.");
                DoneEvent(new ArraySegment<byte>(_Buffer , 0,count));
            }
        }
    }
}