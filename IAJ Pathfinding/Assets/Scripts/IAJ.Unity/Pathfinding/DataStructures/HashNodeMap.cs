using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures
{
    class DictionaryNodeList : IClosedSet
    {
        private Dictionary<Vector3, NodeRecord> NodeRecords { get; set; }

        public void Add(NodeRecord nodeRecord)
        {
            NodeRecords.Add(nodeRecord.node.LocalPosition, nodeRecord);
        }

        public ICollection<NodeRecord> All()
        {
            return NodeRecords.Values;
        }

        public void Initialize()
        {
            NodeRecords = new Dictionary<Vector3, NodeRecord>();
        }

        public void Remove(NodeRecord nodeRecord)
        {
            NodeRecords.Remove(nodeRecord.node.LocalPosition);
        }

        public NodeRecord Search(NodeRecord nodeRecord)
        { 
            if(NodeRecords.Keys.Contains(nodeRecord.node.LocalPosition))
                return NodeRecords[nodeRecord.node.LocalPosition];
            else
                return null;
        }
    }
}
