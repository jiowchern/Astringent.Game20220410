namespace Astringent.Game20220410.Sources
{
    public class EntitiesKeeper
    {
        public readonly System.Collections.Concurrent.ConcurrentDictionary<int , Entity> Entites;
        public EntitiesKeeper()
        {
            Entites = new System.Collections.Concurrent.ConcurrentDictionary<int, Entity>();
        }
    }
}