using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures
{
    public class RightPriorityList : IOpenSet, IComparer<NodeRecord>
    {
        private List<NodeRecord> Open { get; set; }

        public RightPriorityList()
        {
            this.Open = new List<NodeRecord>();    
        }
        public void Initialize()
        {
            this.Open = new List<NodeRecord>();
        }

        public void Replace(NodeRecord nodeToBeReplaced, NodeRecord nodeToReplace)
        {
            this.RemoveFromOpen(nodeToBeReplaced);
            this.AddToOpen(nodeToReplace);
        }

        public NodeRecord GetBestAndRemove()
        {
            var nodeRecord = PeekBest();
            this.Open.Remove(nodeRecord);
            return nodeRecord;
        }

        public NodeRecord PeekBest()
        {
            return this.Open.Last();
        }

        public void AddToOpen(NodeRecord nodeRecord)
        {
            //a little help here
            //is very nice that the List<T> already implements a binary search method
            int index = this.Open.BinarySearch(nodeRecord, this);

            if (index < 0)
            {
                this.Open.Insert(~index, nodeRecord);
            } else this.Open.Insert(index, nodeRecord);
        }

        public void RemoveFromOpen(NodeRecord nodeRecord)
        {
            this.Open.Remove(nodeRecord);
        }

        public NodeRecord SearchInOpen(NodeRecord nodeRecord)
        {
            return Open.FirstOrDefault(x => x.Equals(nodeRecord));
        }

        public ICollection<NodeRecord> All()
        {
            return Open;
        }

        public int CountOpen()
        {
            return Open.Count;
        }

        public int Compare(NodeRecord x, NodeRecord y)
        {
            if (x.fValue == y.fValue) {
                if (x.gValue == y.gValue)
                    return 1;
                else
                    return -(int)Math.Round(x.gValue - y.gValue);
            }
            else
                return -(int)Math.Round(x.fValue - y.fValue);
        }
    }
}
