using Assets.Scripts.IAJ.Unity.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    class FlockVelocityMatching : DynamicVelocityMatch
    {
        public List<DynamicCharacter> flock;
        public float radius;
        public float fanAngle;


        public FlockVelocityMatching()
        {
            this.TimeToTargetSpeed = 0.1f;
        }

        public override MovementOutput GetMovement()
        {
            Vector3 averageVelocity = new Vector3();
            int closeBoids = 0;
           
            foreach(var bird in flock)
            {
                var boid = bird.KinematicData;
                if (Character != boid)
                { 
                    Vector3 direction = boid.position - Character.position;
                    if (direction.magnitude <= radius)
                    {
                        float angle = MathHelper.ConvertVectorToOrientation(direction);
                        float angleDifference = MathHelper.ShortestAngleDifference(Character.orientation, angle);
                        
                        if (Math.Abs(angleDifference) <= fanAngle)
                        {
                            averageVelocity += boid.velocity;
                            closeBoids++;
                        }
                    }
                }
            }
            if (closeBoids == 0)
                return new MovementOutput();
            averageVelocity /= closeBoids;
            Target.velocity = averageVelocity;
            MovingTarget = new KinematicData();
            MovingTarget.velocity=averageVelocity;
            return base.GetMovement();
        }
    }
}
