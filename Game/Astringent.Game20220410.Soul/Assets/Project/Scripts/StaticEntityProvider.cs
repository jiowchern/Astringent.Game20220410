using Unity.Entities;

using UnityEngine;
namespace Astringent.Game20220410.Scripts
{
    public class StaticEntityProvider : MonoBehaviour, IConvertGameObjectToEntity
    {
        public Astringent.Game20220410.Protocol.APPEARANCE Appearance;
        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var e = new Sources.Entity(entity, Appearance);
            
            Dots.Systems.Service.GetWorld().GetExistingSystem<Dots.Systems.Service>().Keeper.Entites.TryAdd(e.Id,e);

        }
    }

}
