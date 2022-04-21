using System;
using Unity.Entities;

namespace Astringent.Game20220410.Dots
{
    [Serializable]
    public struct CollisionEventsReceiverProperties : IComponentData
    {
        public bool UsesCollisionDetails;
    }
}
