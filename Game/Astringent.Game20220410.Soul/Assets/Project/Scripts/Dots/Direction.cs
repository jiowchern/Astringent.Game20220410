using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;

namespace Astringent.Game20220410.Dots
{
    
    public struct Direction : IComponentData
    {
        public float3 Value;
    }
}
