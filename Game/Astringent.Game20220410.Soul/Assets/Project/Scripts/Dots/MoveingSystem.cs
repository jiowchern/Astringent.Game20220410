﻿using Astringent.Game20220410.Scripts;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;


namespace Astringent.Game20220410.Dots.Systems
{
    public partial class MoveingSystem : Unity.Entities.SystemBase 
    {
       
        
        

        protected override void OnCreate()
        {
            
            base.OnCreate();
        }

        protected override void OnDestroy()
        {
         
            base.OnDestroy();
        }
        public MoveingSystem()
        {
         
        }
        
        protected override void OnUpdate()
        {
            var nowTime = Service.GetWorld().Time.ElapsedTime;
            Entities.ForEach((ref Translation translation, in Dots.MoveingState move_state) =>
            {
                var interval = (float)(nowTime - move_state.Data.StartTime);
                translation.Value = move_state.Data.Position + move_state.Data.Vector * interval;
            }).ScheduleParallel();
            
            Entities.WithChangeFilter<Direction>().ForEach((ref Dots.MoveingState move_state, in Direction dir, in Dots.Attributes attributes, in Translation translation) => 
            {
                move_state.Data.StartTime = nowTime;
                move_state.Data.Position = translation.Value;
                move_state.Data.Vector = dir.Value * attributes.Data.Speed;
            }).ScheduleParallel();

        }
    }
}
