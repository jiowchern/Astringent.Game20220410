using System;


namespace Astringent.Game20220410.Protocol
{
    [Unity.Entities.GenerateAuthoringComponent]
    public struct MoveingState : Unity.Entities.IComponentData
    {
        public double StartTime;
        public Unity.Mathematics.float3 Position;
        public Unity.Mathematics.float3 Vector;

       
    }
}
