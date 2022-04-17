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
                Entities.ForEach((ref Past past, in Dots.MoveingState move_state, in Dots.ActorAttributes attributes) =>
                {

                    UnityEngine.Debug.Log("in attr MoveingState");
                    if (Scripts.UnsafeEuqaler.Equal(in past.MoveingState, in move_state.Data))
                        return;

                    writter.Enqueue(new EventsSystemHandler<Protocol.MoveingState>.Data() { State = move_state.Data, Id = attributes.Data.Id }); ;
                }).Schedule(Dependency).Complete();
            });            

            Attributes.Update(writter => {
                Entities.ForEach((ref Past past, in Dots.ActorAttributes attributes) =>
                {
                    UnityEngine.Debug.Log("in attr check");
                    if (Scripts.UnsafeEuqaler.Equal(in past.Attributes, in attributes.Data))
                        return;


                    writter.Enqueue(new EventsSystemHandler<Protocol.Attributes>.Data() { State = attributes.Data, Id = attributes.Data.Id }); ;
                }).Schedule(Dependency).Complete();
            });
        }

      
    }
}
