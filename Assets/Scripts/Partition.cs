using UnityEngine;

namespace DefaultNamespace
{
    public class Partition
    {
        public Partition LeftPartition, RightPartition;
        public Rect PartitionArea;
        
        public Partition(Rect partitionArea)
        {
            this.PartitionArea = partitionArea;
        }

        public bool IsLeaf() => LeftPartition == null && RightPartition == null;

        public bool Split(int minRoomSize, int maxRoomSize)
        {
            if(!IsLeaf()) return false;

            bool splitHorizontal;
            if (PartitionArea.width / PartitionArea.height >= 1.25) splitHorizontal = false;
            else if (PartitionArea.height / PartitionArea.width >= 1.25) splitHorizontal = true;
            else splitHorizontal = Random.Range(0.0f, 1.0f) > 0.5f;

            if (Mathf.Min(PartitionArea.height, PartitionArea.width) / 2 < minRoomSize)
                return false;

            if (splitHorizontal)
            {
                int split = Random.Range(minRoomSize, (int)(PartitionArea.width - minRoomSize));
                
                LeftPartition = new Partition(new Rect(PartitionArea.x, PartitionArea.y, PartitionArea.width, split));
                RightPartition = new Partition(new Rect(PartitionArea.x, PartitionArea.y + split, PartitionArea.width, PartitionArea.height - split));
            }
            else
            {
                int split = Random.Range(minRoomSize, (int)(PartitionArea.height - minRoomSize));
                
                LeftPartition = new Partition(new Rect(PartitionArea.x, PartitionArea.y, split, PartitionArea.height));
                RightPartition = new Partition(new Rect(PartitionArea.x + split, PartitionArea.y, PartitionArea.width - split, PartitionArea.height));
            }
            
            return true;
        }
    }
}