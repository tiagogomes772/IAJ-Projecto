using System.Collections.Generic;
using RAIN.Navigation.Graph;
using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures;
using System;

namespace Assets.Scripts.IAJ.Unity.Pathfinding
{
    public class NodeRecordArray : IOpenSet, IClosedSet
    {
        private List<NavigationGraphNode> nodes;

    public NodeRecordArray(List<NavigationGraphNode> nodes)
            {
                this.nodes = nodes;
            }

        public void AddToClosed(NodeRecord nodeRecord)
        {
            throw new NotImplementedException();
        }

        public void AddToOpen(NodeRecord nodeRecord)
        {
            throw new NotImplementedException();
        }

        public ICollection<NodeRecord> All()
        {
            throw new NotImplementedException();
        }

        public int CountOpen()
        {
            throw new NotImplementedException();
        }

        public NodeRecord GetBestAndRemove()
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        internal object GetNodeRecord(NavigationGraphNode childNode)
        {
            throw new NotImplementedException();
        }

        public NodeRecord PeekBest()
        {
            throw new NotImplementedException();
        }

        public void RemoveFromClosed(NodeRecord nodeRecord)
        {
            throw new NotImplementedException();
        }

        public void RemoveFromOpen(NodeRecord nodeRecord)
        {
            throw new NotImplementedException();
        }

        public void Replace(NodeRecord nodeToBeReplaced, NodeRecord nodeToReplace)
        {
            throw new NotImplementedException();
        }

        public NodeRecord SearchInClosed(NodeRecord nodeRecord)
        {
            throw new NotImplementedException();
        }

        internal void AddSpecialCaseNode(object childNodeRecord)
        {
            throw new NotImplementedException();
        }

        public NodeRecord SearchInOpen(NodeRecord nodeRecord)
        {
            throw new NotImplementedException();
        }
    }
}