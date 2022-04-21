using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Astringent.Game20220410.Dots
{
    [Serializable]
    public struct CollisionEventBufferElement : IBufferElementData
    {
        public Entity Entity;
        public float3 Normal;
        public bool HasCollisionDetails;
        public float3 AverageContactPointPosition;
        public float EstimatedImpulse;
        public PhysicsEventState State;

        public bool _isStale;
    }
}
