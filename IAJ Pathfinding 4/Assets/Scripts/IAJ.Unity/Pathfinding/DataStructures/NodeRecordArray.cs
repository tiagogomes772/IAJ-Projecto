﻿using System;
using System.Collections.Generic;
using RAIN.Navigation.Graph;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures
{
    public class NodeRecordArray : IOpenSet, IClosedSet
    {
        private NodeRecord[] NodeRecords { get; set; }
        private List<NodeRecord> SpecialCaseNodes { get; set; } 
        private NodePriorityHeap Open { get; set; }

        public NodeRecordArray(List<NavigationGraphNode> nodes)
        {
            //this method creates and initializes the NodeRecordArray for all nodes in the Navigation Graph
            this.NodeRecords = new NodeRecord[nodes.Count];
            
            for(int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                node.NodeIndex = i; //we're setting the node Index because RAIN does not do this automatically
                this.NodeRecords[i] = new NodeRecord {node = node, status = NodeStatus.Unvisited};
            }

            this.SpecialCaseNodes = new List<NodeRecord>();

            this.Open = new NodePriorityHeap();
        }

        public NodeRecord GetNodeRecord(NavigationGraphNode node)
        {
            //do not change this method
            //here we have the "special case" node handling
            if (node.NodeIndex == -1)
            {
                for (int i = 0; i < this.SpecialCaseNodes.Count; i++)
                {
                    if (node == this.SpecialCaseNodes[i].node)
                    {
                        return this.SpecialCaseNodes[i];
                    }
                }
                return null;
            }
            else
            {
                return  this.NodeRecords[node.NodeIndex];
            }
        }

        public void AddSpecialCaseNode(NodeRecord node)
        {
            this.SpecialCaseNodes.Add(node);
        }

        void IOpenSet.Initialize()
        {
            this.Open.Initialize();
            //we want this to be very efficient (that's why we use for)
            for (int i = 0; i < this.NodeRecords.Length; i++)
            {
                this.NodeRecords[i].status = NodeStatus.Unvisited;
            }

            this.SpecialCaseNodes.Clear();
        }

        void IClosedSet.Initialize()
        {
            //Heyyeh
        }

        public void AddToOpen(NodeRecord nodeRecord)
        {
            if (!(NodeRecords[nodeRecord.node.NodeIndex].status == NodeStatus.Open))
            {
                nodeRecord.status = NodeStatus.Open;
                NodeRecords[nodeRecord.node.NodeIndex] = nodeRecord;
                
                Open.AddToOpen(nodeRecord);
            }
        }

        public void AddToClosed(NodeRecord nodeRecord)
        {
            nodeRecord.status = NodeStatus.Closed;
            NodeRecords[nodeRecord.node.NodeIndex] = nodeRecord;
        }

        public NodeRecord SearchInOpen(NodeRecord nodeRecord)
        {
            if (NodeRecords[nodeRecord.node.NodeIndex].status == NodeStatus.Open)
            {
                return NodeRecords[nodeRecord.node.NodeIndex];
            }
            else return null;
        }

        public NodeRecord SearchInClosed(NodeRecord nodeRecord)
        {
            if (NodeRecords[nodeRecord.node.NodeIndex].status == NodeStatus.Closed)
            {
                return NodeRecords[nodeRecord.node.NodeIndex];
            }
            else return null;
        }

        public NodeRecord GetBestAndRemove()
        {
            return this.Open.GetBestAndRemove();
        }

        public NodeRecord PeekBest()
        {
            return this.Open.PeekBest();
        }

        public void Replace(NodeRecord nodeToBeReplaced, NodeRecord nodeToReplace)
        {
            NodeRecords[nodeToBeReplaced.node.NodeIndex] = nodeToReplace;
            Open.Replace(nodeToBeReplaced, nodeToReplace);

        }

        public void RemoveFromOpen(NodeRecord nodeRecord)
        {
            if (NodeRecords[nodeRecord.node.NodeIndex].status == NodeStatus.Open)
            {
                this.Open.RemoveFromOpen(nodeRecord);
            }
        }

        public void RemoveFromClosed(NodeRecord nodeRecord)
        {
            if (NodeRecords[nodeRecord.node.NodeIndex].status == NodeStatus.Closed)
            {
                this.Open.RemoveFromOpen(nodeRecord);
            }
        }

        ICollection<NodeRecord> IOpenSet.All()
        {
            return Open.All();
        }

        ICollection<NodeRecord> IClosedSet.All()
        {
            return Open.All();
        }

        public int CountOpen()
        {
            return Open.CountOpen();
        }
    }
}
