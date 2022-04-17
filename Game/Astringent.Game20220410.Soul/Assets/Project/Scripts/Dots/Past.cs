using Unity.Entities;

namespace Astringent.Game20220410.Dots
{
    public struct Past : IComponentData
    {
        public Protocol.Attributes Attributes;
        public Protocol.MoveingState MoveingState;
        public Direction Direction;

    }

    


}