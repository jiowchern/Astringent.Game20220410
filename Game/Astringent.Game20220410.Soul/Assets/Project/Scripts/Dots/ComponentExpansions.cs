using Astringent.Game20220410.Protocol;

namespace Astringent.Game20220410.Dots.Expansions
{
    public static partial class ComponentExpansions
    {
        public static bool Equals(this MoveingState m1, MoveingState m2)
        {
            if (Unity.Mathematics.math.any(m1.Vector != m2.Vector))
                return false;

            if (Unity.Mathematics.math.any(m1.Position != m2.Position))
                return false;

            return true;
        }
    }
}
