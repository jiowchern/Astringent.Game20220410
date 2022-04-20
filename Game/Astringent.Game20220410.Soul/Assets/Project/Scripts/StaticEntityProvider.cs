using Unity.Entities;

using UnityEngine;
namespace Astringent.Game20220410.Scripts
{
    public class StaticEntityProvider : MonoBehaviour, IConvertGameObjectToEntity
    {
        public Astringent.Game20220410.Protocol.APPEARANCE Appearance;
        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
                        
        }
    }

}
