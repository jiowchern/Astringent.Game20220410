namespace Astringent.Game20220410.Protocol
{
    public interface IPlayer
    {
        Regulus.Remote.Property<int> Id { get; }

        Regulus.Remote.Value<int> SetDirection(Unity.Mathematics.float3 dir);
    }
}
