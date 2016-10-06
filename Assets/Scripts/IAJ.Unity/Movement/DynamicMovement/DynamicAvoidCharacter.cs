using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    class DynamicAvoidCharacter
    {
        public const float INF = 1;
        public float maxacceleration;
        public float avoidmargin;
        public float collisionRadius;
        public float maxTimeLookAhead;
        public KinematicData character;
        private KinematicData target;

        public DynamicAvoidCharacter(KinematicData kinematicData)
        {
            this.target = kinematicData;
        }

        public MovementOutput GetMovement()
        {
            MovementOutput output = new MovementOutput();
            Vector3 deltaPos = target.position - character.position;
            Vector3 deltaVel = target.velocity - character.velocity;
            float deltaSpeed = deltaVel.magnitude;            if (deltaSpeed == 0)
                return output;            float timeToClosest = -Vector3.Dot(deltaPos, deltaVel) / (deltaSpeed * deltaSpeed);            if (timeToClosest > maxTimeLookAhead)
                return output;            Vector3 futureDeltaPos = deltaPos + deltaVel * timeToClosest;
            float futureDistance = futureDeltaPos.magnitude;            if (futureDistance > 2 * collisionRadius)
                return new MovementOutput();

            if (futureDistance <= 0 || deltaPos.magnitude < 2 * collisionRadius)
                output.linear = character.position - target.position;
            else
                output.linear = futureDeltaPos *  -1;

            output.linear = output.linear.normalized * maxacceleration;
            return output;
        }
    }
}
