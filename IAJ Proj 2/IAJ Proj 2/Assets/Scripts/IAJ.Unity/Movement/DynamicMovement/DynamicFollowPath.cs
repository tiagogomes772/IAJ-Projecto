using Assets.Scripts.IAJ.Unity.Exceptions;
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
            this.stopRadius = 3f;
            this.maxSpeed = 20;
            this.MaxAcceleration = 20;
            this.CurrentParam = 0;
            this.PathOffset = 0.1f;
        }

        public override MovementOutput GetMovement()
        {
            if(this.Path.PathEnd(this.CurrentParam + 0.4f))
            {
                this.slowRadius = 20f;
            }
            
            if (this.Path.PathEnd(this.CurrentParam))
            {
                return base.GetMovement();
            }
            else
            {
                float targetParam;
                this.CurrentParam = this.Path.GetParam(this.Character.position, this.CurrentParam);
                targetParam = this.CurrentParam + this.PathOffset;
                try
                {
                    this.Target.position = this.Path.GetPosition(targetParam);
                }
                catch(ParamOutOfRangeException e)
                {
                    return base.GetMovement();
                }
                return base.GetMovement();
            }
            
        }
    }
}
