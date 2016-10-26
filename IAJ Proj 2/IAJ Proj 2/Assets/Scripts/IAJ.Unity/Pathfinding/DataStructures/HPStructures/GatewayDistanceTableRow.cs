using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.HPStructures
{
    public class GatewayDistanceTableRow : ScriptableObject
    {
        public GatewayDistanceTableEntry[] entries;

        public GatewayDistanceTableRow(int size)
        {
            entries = new GatewayDistanceTableEntry[size];
            for (int i = 0; i < size; i++)
            {
                entries[i] = new GatewayDistanceTableEntry();
            }
        }
    }
}
