namespace Astringent.Game20220410.Protocol
{
    public interface IActor
    {
        Regulus.Remote.Property<int> Id { get; }

        Regulus.Remote.Property<Vector> Vector { get; }

        Regulus.Remote.Property<Vector> Position { get; }


    }
}
