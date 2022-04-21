using Astringent.Game20220410.Scripts;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;


namespace Astringent.Game20220410.Dots.Systems
{

    
    public partial class MoveingSystem : Unity.Entities.SystemBase 
    {

        protected override void OnCreate()
        {
            
            base.OnCreate();
        }

        protected override void OnDestroy()
        {
         
            base.OnDestroy();
        }
        public MoveingSystem()
        {
         
        }
        
        protected override void OnUpdate()
        {
            var nowTime = Dots.Systems.Service.GetWorld().Time.ElapsedTime;
            /*Entities.ForEach((ref Translation translation, in Dots.MoveingState move_state) =>
            {
                var interval = (float)(nowTime - move_state.Data.StartTime);
                translation.Value = move_state.Data.Position + move_state.Data.Vector * interval;
            }).ScheduleParallel();*/


           

            Dependency = Entities.WithChangeFilter<Direction>().ForEach((ref Unity.Physics.PhysicsVelocity velocity, ref Dots.MoveingState move_state, in Direction dir, in Dots.Attributes attributes, in Translation translation) =>
            {
                move_state.Data.StartTime = nowTime;
                move_state.Data.Position = translation.Value;
                move_state.Data.Vector = dir.Value * move_state.Speed;
                velocity.Linear = move_state.Data.Vector;
                
            }).ScheduleParallel(Dependency);

            var attributes = GetComponentDataFromEntity<Attributes>();
            var elements = GetBufferFromEntity<TriggerEventBufferElement>();

            
     

            Dependency = Entities.ForEach((ref Direction dir, in DynamicBuffer<CollisionEventBufferElement> eles) =>
            {
                foreach (var ele in eles)
                {
                    
                    if (ele.State != PhysicsEventState.Enter)
                        continue;
                    UnityEngine.Debug.Log("2 set dir");
                    if (!attributes.HasComponent(ele.Entity))
                    {
                        continue;
                    }
     
                    UnityEngine.Debug.Log("3 set dir");
                    Attributes com ;
                    if(!attributes.TryGetComponent(ele.Entity, out com))
                        continue;

                    if (com.Data.Appertance != Protocol.APPEARANCE.Barrier)
                        continue;

                    UnityEngine.Debug.Log("set dir");
                    dir = new Direction() { Value = Unity.Mathematics.float3.zero };
                    
                }
            }).Schedule(Dependency);



        }
    }
}
