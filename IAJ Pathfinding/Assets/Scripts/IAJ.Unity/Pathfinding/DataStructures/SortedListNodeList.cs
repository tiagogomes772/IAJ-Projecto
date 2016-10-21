using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures
{
    class SortedListNodeList : IOpenSet
    {
        private SortedList<NodeRecord, NodeRecord> NodeRecords { get; set; }

        public SortedListNodeList()
        {
            NodeRecords = new SortedList<NodeRecord, NodeRecord>(new DuplicateNodeComparer());
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
            try
            {
                this.NodeRecords.Add(nodeRecord, nodeRecord);
            }
            catch(Exception exception)
            {
                int i = 1;
            }
        }

        public void Remove(NodeRecord nodeRecord)
        {
            this.NodeRecords.Remove(nodeRecord);
        }

        public NodeRecord Search(NodeRecord nodeRecord)
        {
            bool exists = this.NodeRecords.Keys.Contains(nodeRecord);

            if(!exists)
                return null;

            else
                return this.NodeRecords[nodeRecord];
        }

        public ICollection<NodeRecord> All()
        {
            return this.NodeRecords.Values;
        }

        public void Replace(NodeRecord nodeToBeReplaced, NodeRecord nodeToReplace)
        {
            this.Remove(nodeToBeReplaced);

            if (!NodeRecords.ContainsKey(nodeToBeReplaced))
            {
                Console.WriteLine("Key to be replaced is not found.");
            }

            this.Remove(nodeToReplace);
            if (!NodeRecords.ContainsKey(nodeToReplace))
            {
                Console.WriteLine("Key to replace is not found.");
            }
            
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
    public class DuplicateNodeFValueComparer : IComparer<float>
    {
        public int Compare(float x, float y)
        {
            
            if(x== y)
                return -1;
            else
                return (int)Math.Round(x - y);
        }
    }

    /// <summary>
    /// If the fvalue of both nodes is equal then to avoid removing duplicates in the Sorted List lets create a convention where we say that node X is allways bigger than node Y
    /// </summary>
    public class DuplicateNodeComparer : IComparer<NodeRecord>
    {
        public int Compare(NodeRecord x, NodeRecord y)
        {
            if (x.fValue == y.fValue)
                return -1;
            else
                return (int)Math.Round(x.fValue - y.fValue);
        }
    }
}
