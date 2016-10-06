using Assets.Scripts.IAJ.Unity.Util;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicWander : DynamicSeek
    {
        public DynamicWander()
        {
            this.Target = new KinematicData();
        }
        public override string Name
        {
            get { return "Wander"; }
        }
        public float TurnAngle { get; private set; }

        public float WanderOffset { get; set; }
        public float WanderRadius { get; set; }
        public float WanderRate { get; set; }



        protected float WanderOrientation { get; set; }

        public override MovementOutput GetMovement()
        {
            WanderRate = MathConstants.MATH_PI_4;
            WanderRadius = 2;
            WanderOffset = 10;


            WanderOrientation += RandomHelper.RandomBinomial() * WanderRate;

            float targetOrientation = WanderOrientation + Character.orientation;

            Vector3 circleCenter = Character.position + WanderOffset * MathHelper.ConvertOrientationToVector(Character.orientation);

            Target.position = circleCenter + WanderRadius * MathHelper.ConvertOrientationToVector(targetOrientation);

            return base.GetMovement();
        }
    }
}
