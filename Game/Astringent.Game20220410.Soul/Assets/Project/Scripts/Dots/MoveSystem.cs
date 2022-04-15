using Astringent.Game20220410.Scripts;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;


namespace Astringent.Game20220410.Dots.Systems
{
    public partial class MoveSystem : Unity.Entities.SystemBase 
    {
        
        public event System.Action<long,Protocol.MoveingState> StateEvent;
        public MoveSystem()
        {
            StateEvent += (i,e) => { };
        }
        
        protected override void OnUpdate()
        {
            var nowTime = Service.GetWorld().Time.ElapsedTime;
            Entities.ForEach((ref Translation translation, ref Dots.MoveingState move_state) =>
            {
                var interval = (float)(nowTime - move_state.Data.StartTime);
                translation.Value = move_state.Data.Position + move_state.Data.Vector * interval;
            }).ScheduleParallel();


            
            Entities.WithoutBurst().ForEach((ref Direction dir,ref Dots.ActorAttributes actor, ref Translation translation,ref Dots.MoveingState move_state) =>
            {
                
                if (actor.Data.Direction.Equals(dir.Value))
                {
                    return;
                }
                actor.Data.Direction = dir.Value;
                move_state.Data.StartTime = nowTime;
                move_state.Data.Position = translation.Value;
                move_state.Data.Vector = dir.Value * actor.Data.Speed;
                StateEvent(actor.Data.Id,move_state.Data);
            }).Run();



          
        }
    }
}
