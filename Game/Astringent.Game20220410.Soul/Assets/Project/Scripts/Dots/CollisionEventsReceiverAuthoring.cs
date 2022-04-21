using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Astringent.Game20220410.Dots
{
    
    public class CollisionEventsReceiverAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public bool UseCollisionDetails;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new CollisionEventsReceiverProperties { UsesCollisionDetails = UseCollisionDetails });
            dstManager.AddBuffer<CollisionEventBufferElement>(entity);
        }
    }
}