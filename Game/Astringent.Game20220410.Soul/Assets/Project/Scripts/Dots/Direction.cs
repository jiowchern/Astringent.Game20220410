using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;
using System;

namespace Astringent.Game20220410.Dots
{
    public struct Direction : IComponentData , System.IEquatable<Direction>
    {
       

        public float3 Value;

        bool IEquatable<Direction>.Equals(Direction other)
        {
            return Unity.Mathematics.math.all(Value == other.Value);
        }
        public static bool operator !=(Direction other1, Direction other2)
        {            
            return math.any(other1.Value != other2.Value);
        }
        public static bool operator== (Direction other1,Direction other2)
        {
            return Unity.Mathematics.math.all(other1.Value == other2.Value);
        }
    }
}
