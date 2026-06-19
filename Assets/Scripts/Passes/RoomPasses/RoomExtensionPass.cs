using System.Collections.Generic;
using Cells;
using Partitions;
using UnityEngine;

namespace Passes.RoomPasses
{
    [CreateAssetMenu(fileName = "Pass", menuName = "Pass/RoomPasses/RoomExtension", order = 1)]
    public class RoomExtensionPass : BaseRoomPass
    {
        private GridManager gridManager;
        
        public override bool SetPass(Partition partition)
        {
            this.gridManager ??= generator.gridManager;
            return CreateExtensionRoom(partition);
        }
        
        public override bool SetPass()
        {
            Debug.LogWarning($"{this} is Missing parameter Partition!");
            return false;
        }
        
        public bool CreateExtensionRoom(Partition partition)
        {
            List<Cell> wallCells = partition.GetCellsOfType(CellType.GROUND);
            if(wallCells.Count == 0) return false;
            
            if(Random.Range(0, 100) < 25) return true;
            
            Cell centerCell = wallCells[Random.Range(0, wallCells.Count)];
            Rect extentionRect = new(centerCell.Position.x - 5/2, centerCell.Position.y - 7/2, 5, 7);

            foreach (var part in generator.partitions)
            {
                if (part == partition) continue;
                if (extentionRect.Overlaps(part.PartitionArea)) return true;
            }
            
            for (int x = (int)extentionRect.x; x < extentionRect.xMax; x++)
            for (int y = (int)extentionRect.y; y < extentionRect.yMax; y++)
            {
                Cell currentCell = this.gridManager.GetCell(new Vector2Int(x, y)) ?? this.gridManager.SetNode(new Cell(new Vector2Int(x,y)));
                
                if(currentCell.Type != CellType.NONE)
                {
                    if (x == extentionRect.xMax - 1 || x == extentionRect.x || y == extentionRect.yMax - 1 || y == extentionRect.y)
                    {
                        if(currentCell.Type == CellType.WALL)
                            currentCell = this.gridManager.ChangeCellType(currentCell, CellType.WALL);
                    }
                    else if (currentCell.Type == CellType.WALL)
                    {
                        currentCell = this.gridManager.ChangeCellType(currentCell, CellType.GROUND);
                    }
                }
                else
                {
                    currentCell = this.gridManager.InstantiateCell(CellType.GROUND, new Vector2Int(x, y), true);
                    partition.Cells.Add(currentCell);
                    
                    if (x == extentionRect.xMax - 1 || x == extentionRect.x || y == extentionRect.yMax - 1 || y == extentionRect.y)
                        currentCell = this.gridManager.ChangeCellType(currentCell, CellType.WALL);
                }
                
                this.gridManager.SetNode(currentCell);
            }

            return true;
        }
    }
}