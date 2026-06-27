using System.Collections.Generic;
using System.Linq;
using Cells;
using HelperScripts;
using Partitions;
using UnityEngine;

namespace Passes.DungeonPass
{
    [CreateAssetMenu(fileName = "Pass", menuName = "Pass/DungeonPass/Hallways", order = 1)]
    public class HallwayPass : BaseDungeonPass
    {
        //private GridManager gridManager;
        private Astar astar;

        public override bool SetPass()
        {
            //this.gridManager = this.generator.gridManager;
            this.astar ??= new Astar(this.generator.gridManager);

            ConnectRooms(generator.startPartition);
            HallwayWalls();

            return true;
        }

        public void ConnectRooms(Partition partition)
        {
            //Get partition door
            List<Cell> partitionDoors = partition.GetCellsOfType(CellType.DOOR);
            if (partitionDoors.Count <= 0)
            {
                Debug.LogWarning($"{partition} does not have door");
                return;
            }
            
            Cell partitionDoor = partitionDoors.First();
            Partition neighbor = CreatePath(partition, partitionDoor.Position);
            
            if (partitionDoors.Count > 1)
                CreatePath(partition, partitionDoors[1].Position, false, true);
            
            if(neighbor == null) return;
            
            //Set connected true
            neighbor.isConnected = true;
            //ConnectRooms(otherPartition)
            ConnectRooms(neighbor);
        }

        private Partition CreatePath(Partition partition, Vector2Int doorPosition, bool isBackToFront = true, bool lookForConnected = false, bool canConnectSelf = false)
        {
            //Get closest non-connected partition
            float closestDistance = 999;
            Partition closestPartition = null;
            foreach (Partition otherPartition in generator.partitions)
            {
                if (!lookForConnected && otherPartition.isConnected) continue;
                if(!canConnectSelf && partition == otherPartition) continue;
                    
                float distance = Vector2.Distance(doorPosition, otherPartition.PartitionArea.center);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPartition = otherPartition;
                }
            }

            if (closestPartition == null)
            {
                Debug.LogWarning($"{partition} can not connect to another partition");
                return null;
            }
            
            //Get partition door
            Cell otherDoor = closestPartition.GetCellsOfType(CellType.DOOR).First();
            if (otherDoor == null)
            {
                Debug.LogWarning($"{partition} does not have door");
                return null;
            }
            
            //Get path from door to otherPartition door
            List<Vector2Int> hallwayCells = isBackToFront 
                ? astar.FindPathToTarget(otherDoor.Position, doorPosition) 
                : astar.FindPathToTarget(doorPosition, otherDoor.Position);
            
            if (hallwayCells.Count == 0)
            {
                Debug.LogWarning($"{partition} Could not find a path from a door");
            }
            
            //Draw path
            var gridManager = this.generator.gridManager;
            foreach (Vector2Int cellPosition in hallwayCells)
            {
                if (gridManager.GetCell(cellPosition).Type == CellType.HALLWAY) break;
                gridManager.InstantiateCell(CellType.HALLWAY,  cellPosition, false);
            }
            
            return closestPartition;
        }

        
        
        public void HallwayWalls()
        {
            var gridManager = this.generator.gridManager;
            
            List<Cell> hallwayTiles = gridManager.GetCellsOfType(CellType.HALLWAY);

            foreach (Cell cell in hallwayTiles)
            {
                foreach (Cell neighbor in gridManager.GetNeighbors(cell.Position, true))
                {
                    if (neighbor.Type != CellType.NONE) continue;
                    gridManager.InstantiateCell(CellType.WALL,  neighbor.Position, true);
                }
            }
        }
    }
}