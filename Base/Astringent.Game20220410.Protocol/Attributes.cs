using System;

namespace Astringent.Game20220410.Protocol
{
    
    public struct Attributes : System.IEquatable<Attributes>
    {
        public int Id;
        public Unity.Mathematics.float3 Direction;
        public Unity.Mathematics.float3 Speed;

        bool IEquatable<Attributes>.Equals(Attributes other)
        {
            if(Id != other.Id)
                return false;
            if (Direction.Equals(other.Direction))
                return false;
            if (Speed.Equals(other.Speed))
                return false;

            return true;
        }
    }
}
