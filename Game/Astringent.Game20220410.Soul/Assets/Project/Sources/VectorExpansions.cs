namespace Astringent.Game20220410.Expansions
{
    public static class VectorExpansions
    {
        public static UnityEngine.Vector3 ToUnity(this Astringent.Game20220410.Protocol.Vector vector)
        {
            return new UnityEngine.Vector3(vector.X, vector.Y, vector.Z);
        }
    }
}
