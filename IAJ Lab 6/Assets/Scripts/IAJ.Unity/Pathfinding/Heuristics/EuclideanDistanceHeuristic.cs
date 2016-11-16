using RAIN.Navigation.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics
{
    public class EuclideanDistanceHeuristic : IHeuristic
    {
        public float H(NavigationGraphNode node, NavigationGraphNode goalNode)
        {
            return (goalNode.LocalPosition - node.LocalPosition).magnitude;
        }
    }
}
