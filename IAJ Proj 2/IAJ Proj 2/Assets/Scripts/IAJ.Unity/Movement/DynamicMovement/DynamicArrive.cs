using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    class DynamicArrive : DynamicVelocityMatch
    {
        public float maxSpeed;
        public float stopRadius;
        public float slowRadius;
        public string name = "Arrive";

        public override MovementOutput GetMovement()
        {
            float targetSpeed;
            Vector3 direction = Target.position - Character.position;
            float distance = direction.magnitude;

            if (distance < stopRadius)
            {
                targetSpeed = 0;
            }

            if (distance > slowRadius)
            {
                targetSpeed = maxSpeed;
            }
            else
            {
                targetSpeed = maxSpeed * (distance / slowRadius);
            }

            Target.velocity = direction.normalized * targetSpeed;

            MovingTarget = this.Target;

            return base.GetMovement();
        }
    }
}
