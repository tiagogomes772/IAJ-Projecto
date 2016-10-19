using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    class Separation: DynamicMovement
    {
        public KinematicData character;
        public List<DynamicCharacter> flock;
        public float maxAcceleration;
        public float radius;
        public float separationFactor;

        public override string Name
        {
            get
            {
                return "Separation";
            }
        }

        public override KinematicData Target
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        

        public override MovementOutput GetMovement()
        {
            MovementOutput output = new MovementOutput();

            foreach(var bird in flock)
            {
                var boid = bird.KinematicData;

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
