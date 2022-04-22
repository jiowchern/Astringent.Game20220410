using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;
using System;

namespace Astringent.Game20220410.Dots
{
    

    public struct Direction : IComponentData
    {
        public float3 PastValue;
        public float3 Value;

        public bool Dirty()
        {
            if(Unity.Mathematics.math.any(Value!= PastValue))
            {
                PastValue = Value;
                return true;
            }
            return false;
        }
    }
}
