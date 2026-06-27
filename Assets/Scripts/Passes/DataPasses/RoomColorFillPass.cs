using System.Collections.Generic;
using Cells;
using HelperScripts;
using Partitions;
using Passes.DungeonPass;
using UnityEngine;

namespace Passes.DataPasses
{
    [CreateAssetMenu(fileName = "Pass", menuName = "Pass/DataPass/RoomColorFill", order = 1)]
    public class RoomColorFillPass : BaseDungeonPass
    {
        //private  GridManager gridManager;
        private FloodFill floodFill;
        
        public override bool SetPass()
        {
            //this.gridManager = this.generator.gridManager;
            this.floodFill ??= new FloodFill();
            
            floodColor(generator.startPartition);
            return true;
        }
        
        public void floodColor(Partition startRoom)
        {
            var gridManager = this.generator.gridManager;
            
            Cell startCell = gridManager.AllNodes[new Vector2Int((int)startRoom.PartitionArea.center.x, (int)startRoom.PartitionArea.center.y)];
            
            Dictionary<Vector2Int, Cell> floodables = new();
            foreach (var cell in gridManager.AllNodes)
            {
                if(cell.Value.Type == CellType.NONE || cell.Value.Type == CellType.WALL) continue;
                floodables.Add(cell.Key, cell.Value);
            }
            
            float steps = 0;
            float amountChecked = 0;
            float floodAmount = floodables.Count;
            floodFill.FloodEffectCells(startCell, floodables, cell =>
            {
                amountChecked++;
                steps = amountChecked / floodAmount;

                cell.GameObject.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, Color.darkBlue, steps);
            });
        }
    }
}