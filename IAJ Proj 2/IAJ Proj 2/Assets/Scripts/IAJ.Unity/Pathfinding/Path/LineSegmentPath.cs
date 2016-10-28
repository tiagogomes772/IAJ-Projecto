using Assets.Scripts.IAJ.Unity.Utils;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.Path
{
    public class LineSegmentPath : LocalPath
    {
        protected Vector3 LineVector;
        private float EndParam = 0.9f;

        public LineSegmentPath(Vector3 start, Vector3 end)
        {
            this.StartPosition = start;
            this.EndPosition = end;
            this.LineVector = end - start;
        }

        public override Vector3 GetPosition(float param)
        {
            return Vector3.Lerp(StartPosition, EndPosition, param);
        }

        public override bool PathEnd(float param)
        {
            if(param >= EndParam)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override float GetParam(Vector3 position, float lastParam)
        {
            return MathHelper.closestParamInLineSegmentToPoint(StartPosition, EndPosition, position);
        }
    }
}
