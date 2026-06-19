using System.Collections.Generic;
using Cells;
using Partitions;
using UnityEngine;

namespace Passes.RoomPasses
{
    public class RoomDoorPass : IGenerationPass
    {
        private readonly Generator generator;
        private readonly GridManager gridManager;
        
        public RoomDoorPass(Generator generator)
        {
            this.generator =  generator;
            this.gridManager = generator.gridManager;
        }
        
        public void DrawDoors(Partition partition) 
        {
            List<Cell> wallCells = partition.GetCellsOfType(CellType.WALL);
            if(wallCells.Count == 0) return;
        
            int doorAmount = Random.Range(0, 100) < 75 ? 1 : 2;
            int maxLoops = 10;
            while (doorAmount > 0)
            {
                Cell wallCell = wallCells[Random.Range(0, wallCells.Count)];
            
                //Check for empty space
                List<Cell> neighbors = this.gridManager.GetNeighbors(wallCell.Position);
            
                bool hasConnectedGround = false;
                bool hasConnectedOOutside = false;
                foreach (var neighbor in neighbors)
                {
                    if(neighbor.Type == CellType.GROUND) hasConnectedGround = true;
                    if(neighbor.Type == CellType.NONE) hasConnectedOOutside = true;
                }
                
                if(!hasConnectedGround || !hasConnectedOOutside) continue;
            
                wallCell = this.gridManager.ChangeCellType(wallCell, CellType.DOOR);
                wallCell.IsOccupied = false;
            
                doorAmount--;
            }
        }
    }
}