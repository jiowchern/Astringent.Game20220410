using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace Astringent.Game20220410.Dots
{
    public struct Direction : IComponentData
    {
        public float3 Value;
    }
}
