using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public class FloodFill
    {
        private Queue <Vector2Int> pathQueue = new();
        
        public void FloodEffect(Cell startCell, Dictionary<Vector2Int, Cell> hallwayCells, Action<Cell> action)
        {
            pathQueue.Clear();
            pathQueue.Enqueue(startCell.Position);

            while (pathQueue.Count > 0)
            {
                Vector2Int currentCell = pathQueue.Peek();
                if(hallwayCells.TryGetValue(currentCell, out Cell cell))
                    action(cell);
                    //cell.GameObject.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, Color.darkBlue, steps);;
                
                
                pathQueue.Dequeue();
                
                if (hallwayCells.ContainsKey(currentCell))
                {
                    hallwayCells.TryGetValue(currentCell + Vector2Int.left, out Cell westCell);
                    if(westCell != null) pathQueue.Enqueue(westCell.Position);
                    
                    hallwayCells.TryGetValue(currentCell + Vector2Int.right, out Cell eastCell);
                    if(eastCell != null) pathQueue.Enqueue(eastCell.Position);
                    
                    hallwayCells.TryGetValue(currentCell + Vector2Int.up, out Cell northCell);
                    if(northCell != null) pathQueue.Enqueue(northCell.Position);
                    
                    hallwayCells.TryGetValue(currentCell + Vector2Int.down, out Cell southCell);
                    if(southCell != null) pathQueue.Enqueue(southCell.Position);
                }
                
                hallwayCells.Remove(currentCell);
            }
        }
    }
}