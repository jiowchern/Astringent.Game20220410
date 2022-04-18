using System;
using Unity.Entities;


namespace Astringent.Game20220410.Dots
{
    [Serializable]
    public struct TriggerEventBufferElement : IBufferElementData
    {
        public Entity Entity;
        public PhysicsEventState State;

        public bool _isStale;
    }
}
