using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using System;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicFollowPath : DynamicArrive
    {
        public Path Path { get; set; }
        public float PathOffset { get; set; }

        public float CurrentParam { get; set; }

        private MovementOutput EmptyMovementOutput { get; set; }


        public DynamicFollowPath(KinematicData character, Path path) 
        {
            this.Target = new KinematicData();
            this.Character = character;
            this.Path = path;
            this.EmptyMovementOutput = new MovementOutput();
            PathOffset = 1.0f;
            CurrentParam = 0.0f;
            //don't forget to set all properties
            //arrive properties
            maxSpeed = 20.0f;
            this.MaxAcceleration = 10.0f;
            stopRadius = 2.0f;
            slowRadius = 4.0f;
    }

        float targetParam;
        public override MovementOutput GetMovement()
        {
            if (Path.PathEnd(CurrentParam))
            {
                return EmptyMovementOutput;
            }
            else
            {
                CurrentParam = Path.GetParam(base.Character.position, CurrentParam);
                targetParam = CurrentParam + PathOffset;
                Target.position = Path.GetPosition(targetParam);
                return base.GetMovement();
            }
        }
    }
}
