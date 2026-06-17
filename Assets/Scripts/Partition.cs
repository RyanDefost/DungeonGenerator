using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class Partition
    {
        public Partition LeftPartition, RightPartition;
        public Rect PartitionArea;

        public Dictionary<Vector2Int, Cell> Cells;
        
        public Rect extention;
        public Room room;
        
        public Partition(Rect partitionArea)
        {
            this.Cells = new Dictionary<Vector2Int, Cell>();
            this.PartitionArea = partitionArea;  
        } 
        
        public bool Split(int minPartitionSize, int maxPartitionSize)
        {
            if(!IsLeaf()) return false;

            //Set split direction
            bool splitHorizontal;
            if (PartitionArea.width / PartitionArea.height >= 1.25) splitHorizontal = false;
            else if (PartitionArea.height / PartitionArea.width >= 1.25) splitHorizontal = true;
            else splitHorizontal = Random.Range(0.0f, 1.0f) > 0.5f;

            if (Mathf.Min(PartitionArea.height, PartitionArea.width) / 2 < minPartitionSize)
                return false;

            if (splitHorizontal)
            {
                int split = Random.Range(minPartitionSize, (int)(PartitionArea.width - minPartitionSize));
                
                LeftPartition = new Partition(new Rect(PartitionArea.x, PartitionArea.y, PartitionArea.width, split));
                RightPartition = new Partition(new Rect(PartitionArea.x, PartitionArea.y + split, PartitionArea.width, PartitionArea.height - split));
            }
            else
            {
                int split = Random.Range(minPartitionSize, (int)(PartitionArea.height - minPartitionSize));
                
                LeftPartition = new Partition(new Rect(PartitionArea.x, PartitionArea.y, split, PartitionArea.height));
                RightPartition = new Partition(new Rect(PartitionArea.x + split, PartitionArea.y, PartitionArea.width - split, PartitionArea.height));
            }
            
            return true;
        }
        
        public bool IsLeaf() => LeftPartition == null && RightPartition == null;
    }
}