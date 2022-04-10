using Unity.Entities;
using Regulus.Remote;
using Astringent.Game20220410.Protocol;

namespace Astringent.Game20220410.Dots
{
    public struct MoveEvent : IComponentData
    {
        Property<Vector> Vector;
        Property<Vector> Position;
    }
}
