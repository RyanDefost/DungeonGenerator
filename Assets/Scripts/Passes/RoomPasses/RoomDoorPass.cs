using System.Collections.Generic;
using Cells;
using Partitions;
using UnityEngine;

namespace Passes.RoomPasses
{
    [CreateAssetMenu(fileName = "Pass", menuName = "Pass/RoomPasses/RoomDoor", order = 1)]
    public class RoomDoorPass : BaseRoomPass
    {
        private GridManager gridManager;
        
        public override bool SetPass(Partition partition)
        {
            this.gridManager ??= generator.gridManager;
            return DrawDoors(partition);
        }
        
        public override bool SetPass()
        {
            Debug.LogWarning($"{this} is Missing parameter Partition!");
            return false;
        }
        
        public bool DrawDoors(Partition partition) 
        {
            List<Cell> wallCells = partition.GetCellsOfType(CellType.WALL);
            if(wallCells.Count == 0) return false;
        
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

            return true;
        }
    }
}