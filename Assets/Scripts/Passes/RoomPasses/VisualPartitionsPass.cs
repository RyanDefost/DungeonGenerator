using Partitions;
using UnityEngine;

namespace Passes.RoomPasses
{
    [CreateAssetMenu(fileName = "Pass", menuName = "Pass/RoomPasses/VisualPartitions", order = 1)]
    public class VisualPartitionsPass : BaseRoomPass
    {
        private GridManager gridManager;
        
        public override bool SetPass(Partition partition)
        {
            this.gridManager ??= generator.gridManager;
            return DisplayPartition(partition);
        }
        
        public override bool SetPass()
        {
            Debug.LogWarning($"{this} is Missing parameter Partition!");
            return false;
        }
        
        public bool DisplayPartition(Partition partition)
        {
            Color color = Random.ColorHSV();
            for (int x = (int)partition.PartitionArea.x; x < partition.PartitionArea.xMax; x++)
            for (int y = (int)partition.PartitionArea.y; y < partition.PartitionArea.yMax; y++)
            {
                GameObject instance = Object.Instantiate(this.gridManager.baseCellPrefab, new Vector3(x, y, -1), Quaternion.identity);
                instance.GetComponent<SpriteRenderer>().color = color;
                instance.transform.SetParent(generator.transform);
            }

            return true;
        }
    }
}