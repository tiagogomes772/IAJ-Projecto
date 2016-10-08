using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    class DynamicFleeRadius : DynamicFlee
    {
        public float radius { get; set; }

        public override string Name
        {
            get { return "FleeRadius"; }
        }

        public override MovementOutput GetMovement()
        {
            if (Target != null)
            {
                float distance = Vector3.Distance(Character.position, Target.position);
                if (distance < radius) {
                    return base.GetMovement();
                } else {
                    Target = null; //Caso seja para deixar de ter efeito para sempre
                }
            }

            return new MovementOutput();
        }
    }
}
