using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    class DynamicAvoidCharacter: DynamicMovement
    {
        public const float INF = 1;
        public float maxacceleration;
        public float avoidmargin;
        public float collisionRadius;
        public float maxTimeLookAhead;
        private KinematicData target;

        public override string Name
        {
            get
            {
                return "AvoidCharacter";
            }
        }

        public override KinematicData Target
        {
            get
            {
                return Target;
            }

            set
            {
                Target = target;
            }
        }

        public float AvoidMargin { get; internal set; }

        public DynamicAvoidCharacter(KinematicData kinematicData)
        {
            this.target = kinematicData;
            
        }

        public override MovementOutput GetMovement()
        {
            MovementOutput output = new MovementOutput();
            Vector3 deltaPos = target.position - Character.position;
            Vector3 deltaVel = target.velocity - Character.velocity;
            float deltaSpeed = deltaVel.magnitude;            if (deltaSpeed == 0)
                return output;            float timeToClosest = -Vector3.Dot(deltaPos, deltaVel) / (deltaSpeed * deltaSpeed);            if (timeToClosest > maxTimeLookAhead)
                return output;            Vector3 futureDeltaPos = deltaPos + deltaVel * timeToClosest;
            float futureDistance = futureDeltaPos.magnitude;            if (futureDistance > 2 * collisionRadius)
                return new MovementOutput();

            if (futureDistance <= 0 || deltaPos.magnitude < 2 * collisionRadius)
                output.linear = Character.position - target.position;
            else
                output.linear = futureDeltaPos *  -1;

            output.linear = output.linear.normalized * maxacceleration;
            return output;
        }
        
    }
}
