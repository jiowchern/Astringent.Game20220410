using System;

namespace Astringent.Game20220410.Protocol
{
    public struct ActorAttributes : Unity.Entities.IComponentData
    {
        public Unity.Mathematics.float3 Direction;
        public Unity.Mathematics.float3 Speed;
    }
    public struct MoveingState : Unity.Entities.IComponentData
    {
        public double StartTime;
        public Unity.Mathematics.float3 Position;
        public Unity.Mathematics.float3 Vector;

       
    }
}
