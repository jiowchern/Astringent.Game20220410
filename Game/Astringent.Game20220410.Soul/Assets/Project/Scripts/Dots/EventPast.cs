using Unity.Entities;
using Unity.Physics;

namespace Astringent.Game20220410.Dots
{
    public struct EventPast : IComponentData
    {
        //public Direction Direction;
        public Protocol.Attributes Attributes;
        public Protocol.MoveingState MoveingState;
        
    }

    


}