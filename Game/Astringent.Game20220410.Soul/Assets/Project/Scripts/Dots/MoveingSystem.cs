using Astringent.Game20220410.Scripts;
using Unity.Entities;
using Unity.Jobs;
using System.Linq;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;


namespace Astringent.Game20220410.Dots.Systems
{

    
    public partial class MoveingSystem : Unity.Entities.SystemBase 
    {

        public static quaternion unityEulerToQuaternion(float3 v)
        {
            return unityEulerToQuaternion(v.y, v.x, v.z);
        }

        public static quaternion unityEulerToQuaternion(float yaw, float pitch, float roll)
        {
            yaw = math.radians(yaw);
            pitch = math.radians(pitch);
            roll = math.radians(roll);

            float rollOver2 = roll * 0.5f;
            float sinRollOver2 = (float)math.sin((double)rollOver2);
            float cosRollOver2 = (float)math.cos((double)rollOver2);
            float pitchOver2 = pitch * 0.5f;
            float sinPitchOver2 = (float)math.sin((double)pitchOver2);
            float cosPitchOver2 = (float)math.cos((double)pitchOver2);
            float yawOver2 = yaw * 0.5f;
            float sinYawOver2 = (float)math.sin((double)yawOver2);
            float cosYawOver2 = (float)math.cos((double)yawOver2);
            float4 result;
            result.w = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2;
            result.x = cosYawOver2 * sinPitchOver2 * cosRollOver2 + sinYawOver2 * cosPitchOver2 * sinRollOver2;
            result.y = sinYawOver2 * cosPitchOver2 * cosRollOver2 - cosYawOver2 * sinPitchOver2 * sinRollOver2;
            result.z = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2;

            return new quaternion(result);
        }

        public static float3 unityQuaternionToEuler(quaternion q2)
        {
            float4 q1 = q2.value;

            float sqw = q1.w * q1.w;
            float sqx = q1.x * q1.x;
            float sqy = q1.y * q1.y;
            float sqz = q1.z * q1.z;
            float unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
            float test = q1.x * q1.w - q1.y * q1.z;
            float3 v;

            if (test > 0.4995f * unit)
            { // singularity at north pole
                v.y = 2f * math.atan2(q1.y, q1.x);
                v.x = math.PI / 2;
                v.z = 0;
                return NormalizeAngles(math.degrees(v));
            }
            if (test < -0.4995f * unit)
            { // singularity at south pole
                v.y = -2f * math.atan2(q1.y, q1.x);
                v.x = -math.PI / 2;
                v.z = 0;
                return NormalizeAngles(math.degrees(v));
            }

            quaternion q3 = new quaternion(q1.w, q1.z, q1.x, q1.y);
            float4 q = q3.value;

            v.y = math.atan2(2f * q.x * q.w + 2f * q.y * q.z, 1 - 2f * (q.z * q.z + q.w * q.w));   // Yaw
            v.x = math.asin(2f * (q.x * q.z - q.w * q.y));                                         // Pitch
            v.z = math.atan2(2f * q.x * q.y + 2f * q.z * q.w, 1 - 2f * (q.y * q.y + q.z * q.z));   // Roll

            return NormalizeAngles(math.degrees(v));
        }

        static float3 NormalizeAngles(float3 angles)
        {
            angles.x = NormalizeAngle(angles.x);
            angles.y = NormalizeAngle(angles.y);
            angles.z = NormalizeAngle(angles.z);
            return angles;
        }

        static float NormalizeAngle(float angle)
        {
            while (angle > 360)
                angle -= 360;
            while (angle < 0)
                angle += 360;
            return angle;
        }

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
            var nowTime = Dots.Systems.Service.GetWorld().Time.ElapsedTime;
            var deltaTime = Dots.Systems.Service.GetWorld().Time.DeltaTime;
    
            var attributes = GetComponentDataFromEntity<Attributes>();

           
            

            Dependency = Entities.ForEach((
                ref Unity.Physics.PhysicsVelocity velocity,
                ref Dots.MoveingState move_state,
                ref Direction dir,
                ref RunningPast past,
                in Translation translation) =>
            {
                if (Unity.Mathematics.math.all(past.Direction == dir.Value))
                    return;
                past.Direction = dir.Value;


                UnityEngine.Debug.Log("change dir");
                
                move_state.Data.StartTime = nowTime;
                move_state.Data.Position = translation.Value;
                move_state.Data.Vector = dir.Value * move_state.Speed;
                velocity.Linear = move_state.Data.Vector;
                

            }).ScheduleParallel(Dependency);
            Dependency = Entities.ForEach((ref Direction dir, in Entity owner, in Attributes attributes, in DynamicBuffer<CollisionEventBufferElement> buffers) => {
                foreach (var buffer in buffers)
                {
                    if (buffer.State == PhysicsEventState.Exit)
                    {
                        continue;
                    }
                    //velocity.Linear = 0;
                    dir.Value = float3.zero;
                    return;
                }

            }).ScheduleParallel(Dependency);
          


        }
    }
}
