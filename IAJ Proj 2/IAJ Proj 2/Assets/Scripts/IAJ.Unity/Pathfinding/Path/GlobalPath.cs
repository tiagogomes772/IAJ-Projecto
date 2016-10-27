using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.Utils;
using RAIN.Navigation.Graph;
using UnityEngine;
using System;

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
            int previousLocalPathIndex = (int)Math.Floor(previousParam);
            float param = this.LocalPaths[previousLocalPathIndex].GetParam(position, previousParam) + previousLocalPathIndex;
            return param;
        }

        public override Vector3 GetPosition(float param)
        {
            int previousLocalPathIndex = (int)Math.Floor(param);
            Vector3 position = position = LocalPaths[previousLocalPathIndex].GetPosition(param - previousLocalPathIndex);
            
            return position;
        }

        public override bool PathEnd(float param)
        {
            float endParam = LocalPaths.Count - 0.1f;
            if (param > endParam)
            {
                return true;
            }
            else
                return false;
        }
    }
}
