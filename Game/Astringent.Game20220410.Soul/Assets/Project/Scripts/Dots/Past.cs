using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace Astringent.Game20220410.Dots
{
    public struct Past : IComponentData
    {
        public Direction Direction;
        public Protocol.Attributes Attributes;
        public Protocol.MoveingState MoveingState;
        internal float3 Position;
    }

    


}