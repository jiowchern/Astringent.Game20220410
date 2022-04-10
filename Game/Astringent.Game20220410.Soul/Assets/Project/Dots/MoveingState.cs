using Unity.Entities;
using Regulus.Remote;
using Astringent.Game20220410.Protocol;

namespace Astringent.Game20220410.Dots
{
    public struct MoveingState : IComponentData
    {
        public Unity.Mathematics.float3 Vector;
        public Unity.Mathematics.float3 Position;
    }
}
