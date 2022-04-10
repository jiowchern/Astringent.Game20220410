namespace Astringent.Game20220410.Protocol
{
    public interface IActor
    {
        Regulus.Remote.Property<int> Id { get; }

        Regulus.Remote.Property<Unity.Mathematics.float3> Vector { get; }

        Regulus.Remote.Property<Unity.Mathematics.float3> Position { get; }


    }
}
