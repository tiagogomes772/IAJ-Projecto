using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    class Separation
    {
        public KinematicData character;
        public List<KinematicData> flock;
        public float maxAcceleration;
        public float radius;
        public float separationFactor;

        public MovementOutput GetMovement()
        {
            MovementOutput output = new MovementOutput();

            foreach(var boid in flock)
            {
                if(boid != character)
                {
                    Vector3 direction = character.position - boid.position;
                    if(direction.magnitude < radius)
                    {
                        float distance = Vector3.Distance(character.position, boid.position);
                        float separationStrength = Math.Min(separationFactor / (distance * distance), maxAcceleration);
                        direction.Normalize();
                        output.linear += direction * separationStrength;
                    }
                }

            }
            if(output.linear.magnitude > maxAcceleration)
            {
                output.linear.Normalize();
                output.linear *= maxAcceleration;
            }

            return output;
        }
    }
}
