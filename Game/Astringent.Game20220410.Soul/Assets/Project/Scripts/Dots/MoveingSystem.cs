using Astringent.Game20220410.Scripts;
using Unity.Entities;
using Unity.Jobs;
using System.Linq;
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
            var deltaTime = Dots.Systems.Service.GetWorld().Time.DeltaTime;
    
            var attributes = GetComponentDataFromEntity<Attributes>();

           
            

            Dependency = Entities.ForEach((                
                ref Unity.Physics.PhysicsVelocity velocity,
                ref Dots.MoveingState move_state,
                ref Direction dir,
                in Translation translation) =>
            {
                

                if(!dir.Dirty())
                    return;

                UnityEngine.Debug.Log("change dir");

                move_state.Data.StartTime = nowTime;
                move_state.Data.Position = translation.Value;
                move_state.Data.Vector = dir.Value * move_state.Speed;
                velocity.Linear = move_state.Data.Vector;

            }).ScheduleParallel(Dependency);
            Dependency = Entities.ForEach((ref Unity.Physics.PhysicsVelocity velocity, in Entity owner, in Attributes attributes, in DynamicBuffer<CollisionEventBufferElement> buffers) => {



                foreach (var buffer in buffers)
                {
                    if (buffer.State != PhysicsEventState.Enter)
                    {
                        continue;
                    }
                    velocity.Linear = 0;
                    return;
                }

            }).ScheduleParallel(Dependency);
            Dependency = Entities.ForEach((

                ref RunningPast past
                , ref Direction direction
                , in Unity.Physics.PhysicsVelocity velocity) =>
            {


                //Unity.Mathematics.int3 l1 = new Unity.Mathematics.int3((int)velocity.Linear.x *10000 , (int)velocity.Linear.y *10000 , (int)velocity.Linear.z *10000);
                //Unity.Mathematics.int3 l2 = new Unity.Mathematics.int3((int)past.VelocityLinear.x * 10000, (int)past.VelocityLinear.y * 10000, (int)past.VelocityLinear.z * 10000);


                var l1 = Unity.Mathematics.math.int3(Unity.Mathematics.math.normalizesafe(velocity.Linear) * 10000);
                var l2 = Unity.Mathematics.math.int3(Unity.Mathematics.math.normalizesafe(direction.Value) * 10000);


                if (Unity.Mathematics.math.all(l1 == l2))
                {
                    return;
                }
                UnityEngine.Debug.Log("reset dir");

                past.VelocityLinear = velocity.Linear;
                direction.Value = Unity.Mathematics.math.normalizesafe(velocity.Linear);

            }).ScheduleParallel(Dependency);


        }
    }
}
