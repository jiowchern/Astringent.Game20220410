using System;
using System.Collections.Generic;
using System.Text;

namespace Astringent.Game20220410.Protocol
{
    public static partial class Provider
    {
        public static Regulus.Remote.IProtocol Create()

        {
            Regulus.Remote.IProtocol protocol = null;
            _Create(ref protocol);
            return protocol;
        }
        [Regulus.Remote.Protocol.Creater]
        static partial void _Create(ref Regulus.Remote.IProtocol provider);
    }
}
