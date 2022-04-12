namespace Astringent.Game20220410.Protocol
{
    public interface IPlayer
    {
        Regulus.Remote.Property<long> Id { get; }

        Regulus.Remote.Value<bool> SetDirection(Unity.Mathematics.float3 dir);
    }
}
