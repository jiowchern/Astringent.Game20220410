using System;


namespace Astringent.Game20220410.Protocol
{


    
    public struct MoveingState : System.IEquatable<MoveingState>
    {
    
        public double StartTime;
        public Unity.Mathematics.float3 Position;
        public Unity.Mathematics.float3 Vector;

        bool IEquatable<MoveingState>.Equals(MoveingState other)
        {
            if (StartTime != other.StartTime)
                return false;
            if (Position.Equals(other.Position))
                return false;
            if (Vector.Equals(other.Vector))
                return false;

            return true;
        }
    }
}
