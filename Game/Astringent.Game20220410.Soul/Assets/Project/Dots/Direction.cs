using UnityEngine;
using Unity.Entities;

namespace Astringent.Game20220410.Dots
{
    public struct Direction : IComponentData
    {
        public Vector3 Value;
    }
}
