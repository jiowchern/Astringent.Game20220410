using Unity.Entities;

using UnityEngine;
using System.Collections.Generic;
using Astringent.Game20220410.Protocol;

namespace Astringent.Game20220410.Scripts
{
    public class GhostPrototypesProvider : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        public GameObject ActorPrefab;
        
        public static Entity ActorEntity { get; private set; }
        
        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {

            var entiry = conversionSystem.GetPrimaryEntity(ActorPrefab);
            dstManager.AddComponent<MoveingState>(entiry);
            dstManager.AddComponent<Dots.Direction>(entiry);
            dstManager.AddComponent<ActorAttributes>(entiry);

            ActorEntity = entiry;
        }

        void IDeclareReferencedPrefabs.DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(ActorPrefab);
        }

    }

}
