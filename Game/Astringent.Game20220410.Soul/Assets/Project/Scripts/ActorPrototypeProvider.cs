using Unity.Entities;

using UnityEngine;
using Astringent.Game20220410.Protocol;
using System.Collections.Generic;

namespace Astringent.Game20220410.Scripts
{
}
namespace Astringent.Game20220410.Scripts
{
    public class ActorPrototypeProvider : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        public GameObject Actor;
        public static Entity Prototype { get; private set; }
        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {

            var prototype = conversionSystem.GetPrimaryEntity(Actor);
            dstManager.AddComponent<MoveingState>(prototype);
            dstManager.AddComponent<Dots.Direction>(prototype);
            dstManager.AddComponent<ActorAttributes>(prototype);
            Prototype = prototype;
        }

        void IDeclareReferencedPrefabs.DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(Actor);
        }

    }

}
