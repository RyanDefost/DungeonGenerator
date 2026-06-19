using System.Collections.Generic;
using Cells;
using Partitions;
using UnityEngine;

namespace Passes.RoomPasses
{
    public class RoomExtensionPass : IGenerationPass
    {
        private readonly Generator generator;
        private readonly GridManager gridManager;
        
        public RoomExtensionPass(Generator generator)
        {
            this.generator =  generator;
            this.gridManager = generator.gridManager;
        }
        
        public void CreateExtensionRoom(Partition partition)
        {
            List<Cell> wallCells = partition.GetCellsOfType(CellType.GROUND);
            if(wallCells.Count == 0) return;
            
            if(Random.Range(0, 100) < 25) return;
            
            Cell centerCell = wallCells[Random.Range(0, wallCells.Count)];
            Rect extentionRect = new(centerCell.Position.x - 5/2, centerCell.Position.y - 7/2, 5, 7);

            foreach (var part in generator.partitions)
            {
                if (part == partition) continue;
                if (extentionRect.Overlaps(part.PartitionArea)) return;
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
        }
    }
}