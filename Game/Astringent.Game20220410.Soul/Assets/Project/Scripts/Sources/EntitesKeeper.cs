namespace Astringent.Game20220410.Sources
{
    public class EntitesKeeper
    {
        public readonly System.Collections.Concurrent.ConcurrentDictionary<int , Entity> Entites;
        public EntitesKeeper()
        {
            Entites = new System.Collections.Concurrent.ConcurrentDictionary<int, Entity>();
        }
    }
}