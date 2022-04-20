using Unity.Entities;

using UnityEngine;
using Astringent.Game20220410.Protocol;
using System.Collections.Generic;
using System;

namespace Astringent.Game20220410.Scripts
{
    public class PrototypesProvider : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        
        public GameObject ActorPrefab;
        

        public static Entity ActorEntity { get; private set; }
        

        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var entiry = conversionSystem.GetPrimaryEntity(ActorPrefab);
            

          

            
            ActorEntity = entiry;

            
        } 

        void IDeclareReferencedPrefabs.DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(ActorPrefab);

        }

        internal static Entity New()
        {
            return Dots.Systems.Service.GetWorld().EntityManager.Instantiate(ActorEntity);
        }
    }

}
