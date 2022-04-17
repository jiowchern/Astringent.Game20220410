using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Jobs;

namespace Astringent.Game20220410.Dots.Systems
{
   

    public partial class TriggerSystem : Unity.Entities.SystemBase 
    {
        [Unity.Burst.BurstCompile]
        public struct TriggrtJob : Unity.Physics.ITriggerEventsJob
        {
            void ITriggerEventsJobBase.Execute(TriggerEvent triggerEvent)
            {
                
                //UnityEngine.Debug.Log($"trigger {triggerEvent.EntityA} : {triggerEvent.EntityB}");
            }
        }
        private StepPhysicsWorld _StepPhysicsWorld;

        protected override void OnCreate()
        {

            
            _StepPhysicsWorld = Scripts.Service.GetWorld().GetOrCreateSystem<Unity.Physics.Systems.StepPhysicsWorld>();
            base.OnCreate();
        }
        
        protected override void OnUpdate()
        {
            
            var job = new TriggrtJob();
            job.Schedule(_StepPhysicsWorld.Simulation ,this.Dependency).Complete();
          
         //   job.Execute  (new TriggerEvent());

        }
    }
}
