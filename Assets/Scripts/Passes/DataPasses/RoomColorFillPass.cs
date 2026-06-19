using System.Collections.Generic;
using Cells;
using HelperScripts;
using Partitions;
using UnityEngine;

namespace Passes.DataPasses
{
    public class RoomColorFillPass : IGenerationPass
    {
        private readonly Generator generator;
        private readonly GridManager gridManager;
        private readonly FloodFill floodFill;
        
        public RoomColorFillPass(Generator generator)
        {
            this.generator =  generator;
            this.gridManager = generator.gridManager;
            
            this.floodFill = new FloodFill();
        }
        
        public void floodColor(Partition startRoom)
        {
            Cell startCell = this.gridManager.AllNodes[new Vector2Int((int)startRoom.PartitionArea.center.x, (int)startRoom.PartitionArea.center.y)];
            
            Dictionary<Vector2Int, Cell> floodables = new();
            foreach (var cell in this.gridManager.AllNodes)
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