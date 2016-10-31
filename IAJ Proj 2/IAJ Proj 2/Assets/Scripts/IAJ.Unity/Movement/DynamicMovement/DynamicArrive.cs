using System;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicArrive : DynamicVelocityMatch
    {
        public override string Name
        {
            get
            {
                return "DynamicArrive";
            }
        }

        public float maxSpeed;
        public float stopRadius;
        public float slowRadius;

        public override MovementOutput GetMovement()
        {
            float targetSpeed;
            Vector3 direction = Target.position - Character.position;
            float distance = direction.sqrMagnitude;

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
                targetSpeed = maxSpeed * (direction.magnitude / slowRadius);
            }

            Target.velocity = direction.normalized * targetSpeed;

            MovingTarget = this.Target;

            return base.GetMovement();
        }
    }
}