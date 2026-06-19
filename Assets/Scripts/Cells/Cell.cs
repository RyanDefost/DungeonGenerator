using System;
using UnityEngine;

namespace Cells
{
    public class Cell
    {
        //todo Could have a partition id to check what partion it is part of.
    
        public Vector2Int Position;
        public GameObject GameObject;
        public CellType Type;

        public float startDistance = 0;
    
        public bool IsOccupied;

        public Cell(Vector2Int position, CellType cellType = CellType.NONE, GameObject gameObject = null,  bool isOccupied = false)
        {
            this.Position = position;
            this.GameObject = gameObject;
            this.Type = cellType;
        
            this.IsOccupied = isOccupied;
        }
    }   
}