using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
namespace Astringent.Game20220410.Dots.Systems
{
    public partial class AddTriggerEventsSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var mgr = Dots.Systems.Service.GetWorld().EntityManager;
            Entities.WithNone<TriggerEventBufferElement>().WithStructuralChanges().ForEach((in Entity entity, in PhysicsCollider collider) => {
                mgr.AddBuffer<TriggerEventBufferElement>(entity);
            }).Run();
        }
    }
}
