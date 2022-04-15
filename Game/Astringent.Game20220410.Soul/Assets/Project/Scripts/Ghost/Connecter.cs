﻿
using Regulus.Network;
using UnityEngine;

namespace Astringent.Game20220410.Scripts
{
    public abstract class Connecter : Regulus.Network.IStreamable
    {
        protected abstract Regulus.Remote.IWaitableValue<int> _Receive(byte[] buffer, int offset, int count);
        Regulus.Remote.IWaitableValue<int> IStreamable.Receive(byte[] buffer, int offset, int count)
        {


            return _Receive(buffer, offset, count);

        }
        protected abstract Regulus.Remote.IWaitableValue<int> _Send(byte[] buffer, int offset, int count);
        Regulus.Remote.IWaitableValue<int> IStreamable.Send(byte[] buffer, int offset, int count)
        {

            return _Send(buffer, offset, count);
        }

        public abstract void Connect(string address);
        public abstract void Disconnect();

        public abstract void Update();
    }
}