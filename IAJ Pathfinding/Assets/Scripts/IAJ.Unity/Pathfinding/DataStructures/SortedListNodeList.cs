using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures
{
    class SortedListNodeList : IOpenSet
    {
        private SortedList<float,NodeRecord> NodeRecords { get; set; }

        public SortedListNodeList()
        {
            this.NodeRecords = new SortedList<float, NodeRecord>(new DuplicateNodeComparator());
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
            this.NodeRecords.Add(nodeRecord.fValue, nodeRecord);
        }

        public void Remove(NodeRecord nodeRecord)
        {
            this.NodeRecords.RemoveAt(this.NodeRecords.IndexOfValue(nodeRecord));
        }

        public NodeRecord Search(NodeRecord nodeRecord)
        {
            int indexOfNode = this.NodeRecords.IndexOfValue(nodeRecord);

            if(indexOfNode == -1)
                return null;

            else
                return this.NodeRecords.Values.ElementAt(indexOfNode);
        }

        public ICollection<NodeRecord> All()
        {
            return this.NodeRecords.Values;
        }

        public void Replace(NodeRecord nodeToBeReplaced, NodeRecord nodeToReplace)
        {
            this.Remove(nodeToBeReplaced);
            this.Add(nodeToReplace);
        }

        public NodeRecord GetBestAndRemove()
        {
            var best = this.PeekBest();
            this.NodeRecords.RemoveAt(0);
            return best;
        }

        public NodeRecord PeekBest()
        {
            //This is stupid and there's probably a better way to do this
            return this.NodeRecords.Values.First();
        }
    
    }

    /// <summary>
    /// Comparator that allow the SortedList to have elements with repeated Keys
    /// If the fvalue of both nodes is equal then to avoid removing duplicates in the Sorted List lets create a convention where we say that node X is allways bigger than node Y
    /// NOTE: This will make the search by key ambiguous as there will be multiple repeated keys but for matters of ordering and finding indexes by object this isn't a problem.
    /// </summary>
    public class DuplicateNodeComparator : IComparer<float>
    {
        public int Compare(float x, float y)
        {
            
            if(x== y)
                return -1;
            else
                return (int)Math.Round(x - y);
        }
    }
}
