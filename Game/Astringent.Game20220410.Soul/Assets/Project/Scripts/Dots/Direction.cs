using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;
using System;

namespace Astringent.Game20220410.Dots
{
    

    public struct Direction : IComponentData
    {
        public float3 Value;
    }
}
