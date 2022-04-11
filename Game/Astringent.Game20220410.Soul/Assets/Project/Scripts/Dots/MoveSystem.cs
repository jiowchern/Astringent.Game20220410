using Astringent.Game20220410.Scripts;
using Unity.Entities;
using Unity.Transforms; 
namespace Astringent.Game20220410.Dots.Systems
{

    
    public class MoveSystem : ComponentSystem
    {
        
        protected override void OnUpdate()
        {
            var nowTime = Service.GetWorld().Time.ElapsedTime;
            Entities.ForEach((ref Direction dir,ref Protocol.ActorAttributes actor, ref Translation translation,ref Protocol.MoveingState move_state) =>
            {
                
                if (actor.Direction.Equals(dir.Value))
                {
                    return;
                }
                actor.Direction = dir.Value;
                move_state.StartTime = nowTime;
                move_state.Position = translation.Value;
                move_state.Vector = dir.Value * actor.Speed;
            });


            
            Entities.ForEach((ref Translation translation, ref Protocol.MoveingState move_state) =>
            {                
                var interval = (float)(nowTime - move_state.StartTime);                
                translation.Value = move_state.Position + move_state.Vector * interval;
            });
        }
    }
}
