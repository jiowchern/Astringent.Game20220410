using Unity.Entities;

using UnityEngine;
using Astringent.Game20220410.Protocol;
using System.Collections.Generic;
namespace Astringent.Game20220410.Scripts
{
    public class SoulPrototypesProvider : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        public GameObject ActorPrefab;

        public static Entity ActorEntity { get; private set; }
        

        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {

            var entiry = conversionSystem.GetPrimaryEntity(ActorPrefab);
            dstManager.AddComponent<Dots.MoveingState>(entiry);
            dstManager.AddComponent<Dots.Direction>(entiry);
            dstManager.AddComponent<Dots.ActorAttributes>(entiry);

            dstManager.AddBuffer<Dots.EntityInView>(entity);

            ActorEntity = entiry;
        } 

        void IDeclareReferencedPrefabs.DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(ActorPrefab);
            
        }
    }

}
