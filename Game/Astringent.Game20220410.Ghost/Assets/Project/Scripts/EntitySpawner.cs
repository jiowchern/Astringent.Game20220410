using UnityEngine;
using UniRx;
using System.Linq;
using Astringent.Game20220410.Protocol;
using System;
using System.Collections.Generic;
using Cinemachine;

namespace Astringent.Game20220410
{
    public class EntitySpawner : AgentReactiveMonoBehaviour 
    {
        
        public GameObject ActorPrefab;
        public Cinemachine.CinemachineVirtualCameraBase FollowCamera;

        readonly System.Collections.Generic.Dictionary<IEntity, Entity> _Entites;
        readonly UniRx.CompositeDisposable _CompositeDisposable;
        public EntitySpawner()
        {
            _Entites = new Dictionary<IEntity, Entity>();
            _CompositeDisposable = new CompositeDisposable();
        }
        new protected void Start()
        {

          //  _Reset();





            base.Start();
        }
        private void _Spawn(IEntity obj,bool is_player)
        {
            
            var actor = GameObject.Instantiate(ActorPrefab).GetComponent<Entity>();
            _Entites.Add(obj, actor);
            actor.Startup(obj);            
            if(is_player)
            {
                FollowCamera.Follow = actor.transform;
                FollowCamera.LookAt = actor.transform;
            }
        }

        

        protected override IEnumerable<IDisposable> _Start(Regulus.Remote.Ghost.IAgent agent)
        {
            _Entites.Clear();
            var addActorObs = 
                              from player in agent.QueryNotifier<IPlayer>().SupplyEvent().First()
                              from actor in agent.QueryNotifier<IEntity>().SupplyEvent()
                              let isPlayer = _Check(player, actor)
                              select new { actor, isPlayer };
            yield return addActorObs.Subscribe((ret) => { _Spawn(ret.actor, ret.isPlayer); });

            var releaseObs = from entity in agent.QueryNotifier<IEntity>().UnsupplyEvent()
                            select entity;
            yield return releaseObs.Subscribe(_Remove);

            /*var removeObs = from player in agent.QueryNotifier<IPlayer>().UnsupplyEvent()
                            select player;

            yield return removeObs.Subscribe(_Reset);*/

        }

        private void _Remove(IEntity obj)
        {
            
            var ent = _Entites[obj];
            _Entites.Remove(obj);
            GameObject.Destroy(ent.gameObject);            
        }

        private void _Reset()
        {
            _CompositeDisposable.Clear();
            var addActorObs = from agent in Observer.First()
                              from player in agent.QueryNotifier<IPlayer>().SupplyEvent().First()
                              from actor in agent.QueryNotifier<IEntity>().SupplyEvent()
                              let isPlayer = _Check(player, actor)
                              select new { actor, isPlayer };
            _CompositeDisposable.Add(addActorObs.Subscribe((ret) => { _Spawn(ret.actor, ret.isPlayer); }));
        }

        private static bool _Check(IPlayer player, IEntity actor)
        {

            return player.Id.Value == actor.Id.Value;
        }

        private void _Reset(IPlayer obj)
        {
            _Reset();
        }
    }

}
