﻿using Unity.Entities;

namespace Astringent.Game20220410.Dots
{
    namespace Systems
    {
    }
    public struct Attributes : IComponentData
    {
        public Protocol.Attributes Data;

        public static implicit operator Attributes(Protocol.Attributes data)
        {
            return new Attributes() { Data = data };
        }
    }

    


}