using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.HPStructures
{
    public class GatewayDistanceTableRow : ScriptableObject
    {
        public GatewayDistanceTableEntry[] entries;
        private int length;

        public GatewayDistanceTableRow(int length)
        {
            this.length = length;
            entries = new GatewayDistanceTableEntry[length];
            for (int i = 0; i < length; i++)
            {
                entries[i] = new GatewayDistanceTableEntry();
            }
        }
    }
}
