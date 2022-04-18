using Unity.Entities;

namespace Astringent.Game20220410.Dots
{
    public struct ParentEntiry : IComponentData
    {
        public Entity Entity;
    }
    public struct MoveingState : IComponentData
    {
        public Protocol.MoveingState Data;

        public static implicit operator MoveingState(Protocol.MoveingState data)
        {
            return new MoveingState() { Data = data };
        }
    }

    


}