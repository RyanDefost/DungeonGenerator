using Cells;
using Partitions;
using UnityEngine;

namespace Passes.RoomPasses
{
    [CreateAssetMenu(fileName = "Pass", menuName = "Pass/RoomPasses/BaseRoom", order = 1)]
    public class RoomBasePass : BaseRoomPass
    {
        private GridManager gridManager;

        
        public override bool SetPass(Partition partition)
        {
            this.gridManager ??= generator.gridManager;
            return CreateBaseRoom(partition);
        }
        
        public override bool SetPass()
        {
            Debug.LogWarning($"{this} is Missing parameter Partition!");
            return false;
        }
        
        public bool CreateBaseRoom(Partition partition)
        {
            Rect partitionArea = partition.PartitionArea;
            float roomWidth = (int)Random.Range(partitionArea.width / generator.sizeOffset, partitionArea.width - 2);
            float roomHeight = (int)Random.Range(partitionArea.height / generator.sizeOffset, partitionArea.height - 2);
            int roomX = (int)Random.Range(1, partitionArea.width - roomWidth - 1);
            int roomY = (int)Random.Range(1, partitionArea.height - roomHeight - 1);
        
            Rect roomArea = new(partitionArea.x + roomX, partitionArea.y + roomY, roomWidth, roomHeight);
        
            if(roomArea.width < generator.minRoomWith || roomArea.height < generator.minRoomHeight) return false;
        
            for (int x = (int)roomArea.x; x < roomArea.xMax; x++)
            for (int y = (int)roomArea.y; y < roomArea.yMax; y++)
            {
                Cell cell = this.gridManager.InstantiateCell(CellType.GROUND, new Vector2Int(x, y), true);
                
                if (x == roomArea.xMax - 1 || x == roomArea.x || y == roomArea.yMax - 1 || y == roomArea.y)
                    cell = this.gridManager.ChangeCellType(cell, CellType.WALL);
            
                partition.Cells.Add(cell);
            }

            return true;
        }
    }
}