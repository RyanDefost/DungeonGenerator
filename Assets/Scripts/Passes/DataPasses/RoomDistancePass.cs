using System.Collections.Generic;
using System.Linq;
using Cells;
using HelperScripts;
using Partitions;
using Passes.DungeonPass;
using UnityEngine;

namespace Passes.DataPasses
{
    [CreateAssetMenu(fileName = "Pass", menuName = "Pass/DataPass/RoomDistance", order = 1)]
    public class RoomDistancePass : BaseDungeonPass
    {
        private  GridManager gridManager;
        private  FloodFill floodFill;
        
        public override bool SetPass()
        {
            this.gridManager ??= this.generator.gridManager;
            this.floodFill ??= new FloodFill();
            
            AssignDistance(this.generator.startPartition);
            return true;
        }
        
        public void AssignDistance(Partition startRoom)
        {
            Cell startCell = this.gridManager.AllNodes[new Vector2Int(
                (int)startRoom.PartitionArea.center.x, 
                (int)startRoom.PartitionArea.center.y)
            ];
            
            Dictionary<Vector2Int, Cell> floodableCells = new();
            foreach (KeyValuePair<Vector2Int, Cell> cell in this.gridManager.AllNodes)
            {
                if(cell.Value.Type is CellType.NONE or CellType.WALL) continue;
                floodableCells.Add(cell.Key, cell.Value);
            }
            
            float amountChecked = 0;
            // float distanceFromStart = 0;
            float floodAmount = floodableCells.Count;
            floodFill.FloodEffectCells(startCell, floodableCells, cell =>
            {
                amountChecked++;
                cell.startDistance = amountChecked / floodAmount;
            });

            foreach (var partition in this.generator.partitions)
            {
                float lowestValue =  float.MaxValue;
                foreach (Cell partitionCell in partition.Cells.Where(partitionCell => partitionCell.startDistance < lowestValue))
                {
                    if(partitionCell.Type == CellType.GROUND)
                        partition.distanceValue = partitionCell.startDistance;
                }
            }
        }
    }
}