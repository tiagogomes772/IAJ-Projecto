using Assets.Scripts.IAJ.Unity.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    // TODO find SuperClass
    class Cohesion : DynamicSeek
    {
        public KinematicData character;
        public KinematicData target;
        public List<KinematicData> flock;
        public float radius;
        public float fanAngle;

        private float ShortestAngleDifference(float source, float target)
        {
            float delta = target - source;
            if (delta > Math.PI)
                delta -= 360;
            else if (delta < Math.PI)
                delta += 360;

            return delta;
        }

        public override MovementOutput GetMovement()
        {
            Vector3 massCenter = new Vector3();
            float closeBoids = 0;
            foreach(var boid in flock)
            {
                if(character != boid)
                {
                    Vector3 direction = boid.position - character.position;

                    if(direction.magnitude <= radius)
                    {
                        float angle = MathHelper.ConvertVectorToOrientation(direction);
                        float angleDifference = ShortestAngleDifference(character.orientation, angle);

                        if(Math.Abs(angleDifference) <= fanAngle)
                        {
                            massCenter += boid.position;
                            closeBoids++;
                        }
                    }
                }
            }
            if (closeBoids == 0)
                return new MovementOutput();
            massCenter /= closeBoids;
            target.position = massCenter;

            //TODO: Understand which superclass belongs
            return base.GetMovement();
        }

    }
}
