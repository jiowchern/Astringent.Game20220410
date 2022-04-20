using System;
using Unity.Entities;
using Unity.Jobs;


namespace Astringent.Game20220410.Dots.Systems
{


    public partial class EventsSystem : Unity.Entities.SystemBase
    {
        
        public EventsSystemHandler<Protocol.Attributes> Attributes;
        public EventsSystemHandler<Protocol.MoveingState> MoveingState;
        public EventsSystemHandler<TriggerEventBufferElement> TriggerEventBufferElement;



        protected override void OnCreate()
        {
            TriggerEventBufferElement = new EventsSystemHandler<TriggerEventBufferElement>();
            Attributes = new EventsSystemHandler<Protocol.Attributes>();
            MoveingState = new EventsSystemHandler<Protocol.MoveingState>();
            


            base.OnCreate();
        }



        protected override void OnDestroy()
        {
          
            Attributes.Dispose();
            MoveingState.Dispose();
            TriggerEventBufferElement.Dispose();
            base.OnDestroy();
        }
        protected override void OnUpdate()
        {
            
            MoveingState.Update((writter) => {
                Entities.WithChangeFilter<Dots.MoveingState>().ForEach((in Entity owner,in Dots.MoveingState move_state) =>
                {
                    writter.Enqueue(new EventsSystemHandler<Protocol.MoveingState>.Data() { State = move_state.Data, Owner = owner }); ;
                }).Schedule(Dependency).Complete();
            });            

            Attributes.Update(writter => {
                Entities.WithChangeFilter<Dots.Attributes>().ForEach((in Entity owner, in Dots.Attributes attributes) =>
                {
                    writter.Enqueue(new EventsSystemHandler<Protocol.Attributes>.Data() { State = attributes.Data,  Owner = owner }); 
                }).Schedule(Dependency).Complete();
            });

            TriggerEventBufferElement.Update(writter =>
            {
                Entities.WithChangeFilter<TriggerEventBufferElement>().ForEach((in Entity owner, in DynamicBuffer<TriggerEventBufferElement> elements) =>
                {
                    foreach (var element in elements)
                    {
                        if(!element._isStale)
                            writter.Enqueue(new EventsSystemHandler<TriggerEventBufferElement>.Data() { State = element, Owner = owner }); ;
                    }
                    
                }).Schedule(Dependency).Complete();
            });
        }

      
    }
}
