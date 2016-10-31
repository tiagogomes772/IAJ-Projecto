using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.Utils;
using RAIN.Navigation.Graph;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.Path
{
    public class GlobalPath : Path
    {
        public List<NavigationGraphNode> PathNodes { get; protected set; }
        public List<Vector3> PathPositions { get; protected set; } 
        public bool IsPartial { get; set; }
        public float Length { get; set; }
        public List<LocalPath> LocalPaths { get; protected set; } 


        public GlobalPath()
        {
            this.PathNodes = new List<NavigationGraphNode>();
            this.PathPositions = new List<Vector3>();
            this.LocalPaths = new List<LocalPath>();
        }

        public void CalculateLocalPathsFromPathPositions(Vector3 initialPosition)
        {
            Vector3 previousPosition = initialPosition;
            for (int i = 0; i < this.PathPositions.Count; i++)
            {

                if (!previousPosition.Equals(this.PathPositions[i]))
                {
                    this.LocalPaths.Add(new LineSegmentPath(previousPosition, this.PathPositions[i]));
                    previousPosition = this.PathPositions[i];
                }
            }
        }

        public override float GetParam(Vector3 position, float previousParam)
        {
            float toRetParam = 0.0f;
            for (int i = 0; i < LocalPaths.Count + 1; i++)
            {
                if (i > previousParam)
                {
                    toRetParam = LocalPaths[i - 1].GetParam(position, previousParam);
                    toRetParam += i - 1;
                    break;
                }
            }
            return toRetParam;
        }

        public override Vector3 GetPosition(float param)
        {
            Vector3 toRetVector = Vector3.zero;
            if (param + 1 > LocalPaths.Count)
            {
                toRetVector = LocalPaths[LocalPaths.Count - 1].GetPosition(1);
                return toRetVector;
            }

            for (int i = 0; i < LocalPaths.Count + 1; i++)
            {
                if (i > param)
                {
                    toRetVector = LocalPaths[i - 1].GetPosition(param - (i - 1));
                    break;
                }
            }
            return toRetVector;
        }

        public override bool PathEnd(float param)
        {
            float endParam = LocalPaths.Count - 0.2f;
            if (param >= endParam)
            {
                return true;
            }
            else
                return false;
        }
    }
}
