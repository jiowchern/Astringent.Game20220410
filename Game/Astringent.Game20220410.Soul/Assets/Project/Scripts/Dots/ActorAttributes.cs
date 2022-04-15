using Unity.Entities;

namespace Astringent.Game20220410.Dots
{
    public struct MoveingState : IComponentData
    {
        public Protocol.MoveingState Data;

        public static implicit operator MoveingState(Protocol.MoveingState data)
        {
            return new MoveingState() { Data = data };
        }
    }
    public struct ActorAttributes : IComponentData
    {
        public Protocol.ActorAttributes Data;

        public static implicit operator ActorAttributes(Protocol.ActorAttributes data)
        {
            return new ActorAttributes() { Data = data };
        }
    }

    


}