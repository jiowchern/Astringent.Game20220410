namespace Astringent.Game20220410.Protocol
{
    
    public interface IEntity
    {
        Regulus.Remote.Property<int> Id { get; }

        Regulus.Remote.Property<MoveingState> MoveingState { get; }

        Regulus.Remote.Property<Attributes> Attributes { get; }
        

    }
}
