namespace Astringent.Game20220410.Protocol
{
    public struct ActorAttributes : Unity.Entities.IComponentData
    {
        public long Id;
        public Unity.Mathematics.float3 Direction;
        public Unity.Mathematics.float3 Speed;
    }
}
