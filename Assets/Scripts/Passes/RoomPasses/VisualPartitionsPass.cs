using Partitions;
using UnityEngine;

namespace Passes.RoomPasses
{
    public class VisualPartitionsPass : IGenerationPass
    {
        private readonly Generator generator;
        private readonly GridManager gridManager;

        public VisualPartitionsPass(Generator generator)
        {
            this.generator =  generator;
            this.gridManager = generator.gridManager;
        }
        
        public void DisplayPartition(Partition partition)
        {
            Color color = Random.ColorHSV();
            for (int x = (int)partition.PartitionArea.x; x < partition.PartitionArea.xMax; x++)
            for (int y = (int)partition.PartitionArea.y; y < partition.PartitionArea.yMax; y++)
            {
                GameObject instance = Object.Instantiate(this.gridManager.baseCellPrefab, new Vector3(x, y, -1), Quaternion.identity);
                instance.GetComponent<SpriteRenderer>().color = color;
                instance.transform.SetParent(generator.transform);
            }
        }
    }
}