namespace Astringent.Game20220410.Protocol
{
    
    public interface IActor
    {
        Regulus.Remote.Property<int> Id { get; }

        Regulus.Remote.Property<MoveingState> MoveingState { get; }

        Regulus.Remote.Property<ActorAttributes> Attributes { get; }
        

    }
}
