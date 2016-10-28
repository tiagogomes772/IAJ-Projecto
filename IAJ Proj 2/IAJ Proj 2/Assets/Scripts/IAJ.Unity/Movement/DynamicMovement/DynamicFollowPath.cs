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
            //don't forget to set all properties
            //arrive properties
            this.slowRadius = 5;
            this.stopRadius = 1;
            this.maxSpeed = 20;
            this.MaxAcceleration = 20;
            this.CurrentParam = 0;
            this.PathOffset = 0.2f;
        }

        public override MovementOutput GetMovement()
        {
            float targetParam;
            this.CurrentParam = this.Path.GetParam(this.Character.position, this.CurrentParam);
            targetParam = this.CurrentParam + this.PathOffset;
            if (this.Path.PathEnd(targetParam))
            {
                return EmptyMovementOutput;
            }
            else
            {
                
                this.Target.position = this.Path.GetPosition(targetParam);
                return base.GetMovement();
            }
            
        }
    }
}
