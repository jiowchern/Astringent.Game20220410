using Regulus.Network;
using Regulus.Remote;
using Regulus.Utility;
using System;

namespace Astringent.Websocket2Tcpsocket.Runner
{
    internal　 class TranportSendState : Regulus.Utility.IStatus
    {
        private readonly IStreamable _Stream;
        private readonly ArraySegment<byte> _Buffer;
        private IAwaitable<int> _Waiter;
        int _Offset;
        public event System.Action DoneEvent;
        public TranportSendState(IStreamable stream, System.ArraySegment<byte> buffer)
        {
            _Offset = 0;
            this._Stream = stream;
            this._Buffer = buffer;
        }
        void IStatus.Enter()
        {
            _Waiter = _Stream.Send(_Buffer.Array, _Buffer.Offset, _Buffer.Count).GetAwaiter();
        }

        void IStatus.Leave()
        {
            
        }

        void IStatus.Update()
        {
            if(_Waiter.IsCompleted)
            {
                var count = _Waiter.GetResult();
                
                if ( count + _Buffer.Offset < _Buffer.Count)
                {
                    _Offset += count;
                    Regulus.Utility.Log.Instance.WriteInfo($"send partial {count}byte.");
                    _Waiter = _Stream.Send(_Buffer.Array, _Buffer.Offset + _Offset, _Buffer.Count).GetAwaiter();
                }
                else
                {
                    Regulus.Utility.Log.Instance.WriteInfo($"send {_Buffer.Count}byte.");
                    DoneEvent();
                }
            }
        }
    }
}