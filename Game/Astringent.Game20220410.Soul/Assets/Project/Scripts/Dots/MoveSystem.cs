using Unity.Entities;
using Unity.Transforms; 
namespace Astringent.Game20220410.Dots.Systems
{

    [Unity.Entities.WorldSystemFilter(WorldSystemFilterFlags.All)]
    public partial class MoveSystem : SystemBase
    {
        
        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;
            Entities.ForEach((ref Direction dir,ref Translation translation,ref MoveingState move) =>
            {
                translation.Value += dir.Value * deltaTime;
                move.Vector = dir.Value;
                move.Position = translation.Value;                
                
            }).ScheduleParallel();
        }
    }
}
