using Unity.Entities;

namespace Astringent.Game20220410.Dots
{
    public struct Past : IComponentData
    {
        public Direction Direction;
        public DynamicBuffer<TriggerEventBufferElement> Elements;
    }

    


}