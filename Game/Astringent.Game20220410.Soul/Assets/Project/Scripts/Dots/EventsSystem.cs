using System;
using Unity.Entities;
using Unity.Jobs;


namespace Astringent.Game20220410.Dots.Systems
{


    public partial class EventsSystem : Unity.Entities.SystemBase
    {
        
        public EventsSystemHandler<Protocol.Attributes> Attributes;
        public EventsSystemHandler<Protocol.MoveingState> MoveingState;
       


        protected override void OnCreate()
        {
            Attributes = new EventsSystemHandler<Protocol.Attributes>();
            MoveingState = new EventsSystemHandler<Protocol.MoveingState>();
            
            base.OnCreate();
        }



        protected override void OnDestroy()
        {
            Attributes.Dispose();
            MoveingState.Dispose();
            base.OnDestroy();
        }
        protected override void OnUpdate()
        {
            
            MoveingState.Update((writter) => {
                Entities.WithChangeFilter<Dots.MoveingState>().ForEach((in Dots.MoveingState move_state, in Dots.ActorAttributes attributes) =>
                {
                    writter.Enqueue(new EventsSystemHandler<Protocol.MoveingState>.Data() { State = move_state.Data, Id = attributes.Data.Id }); ;
                }).Schedule(Dependency).Complete();
            });            

            Attributes.Update(writter => {
                Entities.WithChangeFilter<Dots.ActorAttributes>().ForEach((in Dots.ActorAttributes attributes) =>
                {
                    writter.Enqueue(new EventsSystemHandler<Protocol.Attributes>.Data() { State = attributes.Data, Id = attributes.Data.Id }); ;
                }).Schedule(Dependency).Complete();
            });
        }

      
    }
}
