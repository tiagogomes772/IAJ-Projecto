using RAIN.Navigation.Graph;
using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.HPStructures;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics
{
    public class GatewayHeuristic : IHeuristic
    {
        public ClusterGraph ClusterGraph { get; set; }

        public GatewayHeuristic(ClusterGraph clusterGraph)
        {
            this.ClusterGraph = clusterGraph;
        }

        public float H(NavigationGraphNode node, NavigationGraphNode goalNode)
        {
            //for now just returns the euclidean distance
            Cluster startCluster = ClusterGraph.Quantize(node);
            Cluster goalCluster = ClusterGraph.Quantize(goalNode);

            if (startCluster == null)
            {
                return EuclideanDistance(node.LocalPosition, goalNode.LocalPosition);
            }
            if (startCluster.Equals(goalCluster))
            {
                return EuclideanDistance(node.LocalPosition, goalNode.LocalPosition);
            }
            else
            {
                float minDist = float.MaxValue;
                float currentDist = 0f;

                foreach (Gateway startGateway in startCluster.gateways)
                //for (int j = 0; j < startCluster.gateways.Count; j++)
                {
                    //Gateway startGateway = startCluster.gateways[j];
                    currentDist = 0f;
                    currentDist += EuclideanDistance(node.LocalPosition, startGateway.center);

                    foreach (Gateway endGateway in goalCluster.gateways)
                    //for (int i = 0; i < goalCluster.gateways.Count; i++)
                    {
                        //Gateway endGateway = goalCluster.gateways[i];
                        float? shortestDistance = ClusterGraph.gatewayDistanceTable[startGateway.id].entries[endGateway.id].shortestDistance;
                        currentDist += (shortestDistance == null) ? 0 : (float)shortestDistance;
                        //currentDist += EuclideanDistance(node.LocalPosition, endGateway.center);
                        currentDist += EuclideanDistance(endGateway.center, goalNode.LocalPosition);
                        if (minDist > currentDist)
                        {
                            minDist = currentDist;
                        }
                    }
                }
                return minDist;
            }
        }

        public float EuclideanDistance(Vector3 startPosition, Vector3 endPosition)
        {
            return (endPosition - startPosition).magnitude;
        }
    }
}
