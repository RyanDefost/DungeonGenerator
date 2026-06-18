using System;
using UnityEngine;

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
    
    /*public static bool operator ==(Cell c1, Cell c2)
    {
        return (c1.Position == c2.Position && c1.GameObject == c2.GameObject);
    }

    public static bool operator !=(Cell c1, Cell c2)
    {
        return !(c1 == c2);
    }

    public static bool operator ==(Cell c1, Vector2Int v2)
    {
        return (c1.Position.x == v2.x && c1.Position.y == v2.y);
    }

    public static bool operator !=(Cell c1, Vector2Int v2)
    {
        return !(c1 == v2);
    }*/
}