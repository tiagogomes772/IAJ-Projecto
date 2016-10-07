using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    class DynamicAvoidObstacle : DynamicSeek
    {
        public Collider collisionDetector;
        public float avoidMargin, maxLookAhead, whiskerAngle, whiskerLookAhead;

        public override string Name
        {
            get { return "AvoidObstacle"; }
        }

        public DynamicAvoidObstacle(GameObject obstacle)
        {
            collisionDetector = obstacle.GetComponent<Collider>();
        }

        public override MovementOutput GetMovement()
        {
            if(Character.velocity.magnitude == 0)
                return new MovementOutput();
            
            Vector3 leftRayDirection = Quaternion.Euler(0, whiskerAngle, 0) * Character.velocity;
            Vector3 rightRayDirection = Quaternion.Euler(0, -whiskerAngle, 0) * Character.velocity;

            Ray middleRay = new Ray(Character.position, Character.velocity.normalized);
            Ray whiskerLeftRay = new Ray(Character.position, leftRayDirection.normalized);
            Ray whiskerRightRay = new Ray(Character.position, rightRayDirection.normalized);

            Debug.DrawRay(Character.position, Character.velocity.normalized * maxLookAhead, Color.red);
            Debug.DrawRay(Character.position, leftRayDirection.normalized * whiskerLookAhead, Color.magenta);
            Debug.DrawRay(Character.position, rightRayDirection.normalized * whiskerLookAhead, Color.magenta);

            RaycastHit hit;

            if(collisionDetector.Raycast(middleRay, out hit, maxLookAhead) || 
               collisionDetector.Raycast(whiskerLeftRay, out hit, maxLookAhead) || 
               collisionDetector.Raycast(whiskerRightRay, out hit, maxLookAhead))
            {
                Target.position = hit.point + hit.normal * avoidMargin;
                return base.GetMovement();
            }
            return new MovementOutput();

            
        }
    }

}
