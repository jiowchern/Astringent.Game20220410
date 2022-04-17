using Unity.Entities;

namespace Astringent.Game20220410.Dots
{
    public struct ActorAttributes : IComponentData
    {
        public Protocol.Attributes Data;

        public static implicit operator ActorAttributes(Protocol.Attributes data)
        {
            return new ActorAttributes() { Data = data };
        }
    }

    


}