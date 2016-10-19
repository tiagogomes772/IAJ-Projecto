using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures
{
    class OrderOnAddNodeList: IOpenSet
    {
        private List<NodeRecord> NodeRecords { get; set; }

        public OrderOnAddNodeList()
        {
            this.NodeRecords = new List<NodeRecord>();
        }

        public void Initialize()
        {
            this.NodeRecords.Clear();
        }

        public int Count()
        {
            return this.NodeRecords.Count;
        }

        public void Add(NodeRecord nodeRecord)
        {
            this.NodeRecords.Add(nodeRecord);
            this.NodeRecords = this.NodeRecords.OrderBy(x => x.fValue).ToList();
        }

        public void Remove(NodeRecord nodeRecord)
        {
            this.NodeRecords.Remove(nodeRecord);
        }

        public NodeRecord Search(NodeRecord nodeRecord)
        {
            //here I cannot use the == comparer because the nodeRecord will likely be a different computational object
            //and therefore pointer comparison will not work, we need to use Equals
            //LINQ with a lambda expression
            return this.NodeRecords.FirstOrDefault(n => n.Equals(nodeRecord));
        }

        public ICollection<NodeRecord> All()
        {
            return this.NodeRecords;
        }

        public void Replace(NodeRecord nodeToBeReplaced, NodeRecord nodeToReplace)
        {
            nodeToBeReplaced.parent = nodeToReplace.parent;
            nodeToBeReplaced.fValue = nodeToReplace.fValue;
            nodeToBeReplaced.gValue = nodeToReplace.gValue;
            nodeToBeReplaced.hValue = nodeToReplace.hValue;
            this.NodeRecords = this.NodeRecords.OrderBy(x => x.fValue).ToList();
        }

        public NodeRecord GetBestAndRemove()
        {
            var best = this.PeekBest();
            this.NodeRecords.RemoveAt(0);
            return best;
        }

        public NodeRecord PeekBest()
        {
            //welcome to LINQ guys, for those of you that remember LISP from the AI course, the LINQ Aggregate method is the same as lisp's Reduce method
            //so here I'm just using a lambda that compares the first element with the second and returns the lowest
            //by applying this to the whole list, I'm returning the node with the lowest F value.
            return this.NodeRecords[0];
        }
    }
}
