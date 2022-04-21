using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;



namespace Astringent.Game20220410.Dots
{
    
    
    public class TriggerEventsReceiverAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddBuffer<TriggerEventBufferElement>(entity);
        }
    }

}
