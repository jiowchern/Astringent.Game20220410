using Unity.Entities;

namespace Astringent.Game20220410.Dots
{
    
    public struct Attributes : IComponentData
    {
        public int Id;
        public Protocol.Attributes Data;

        public static implicit operator Attributes(Protocol.Attributes data)
        {
            return new Attributes() { Data = data };
        }
    }

    


}