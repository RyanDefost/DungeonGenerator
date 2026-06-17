using System.Collections.Generic;
using UnityEngine;

public enum CellType
{
    NONE = 0,
    GROUND,
    DOOR,
    WALL,
    HALLWAY
}

public struct Cell
{
    public Vector2Int Position;
    public GameObject GameObject;
    public CellType Type;
    
    public bool IsOccupied;

    public Cell(Vector2Int position, CellType cellType = CellType.NONE, GameObject gameObject = null,  bool isOccupied = false)
    {
        this.Position = position;
        this.GameObject = gameObject;
        this.Type = cellType;
        
        this.IsOccupied = isOccupied;
    }
        
    public static bool operator ==(Cell c1, Cell c2) 
    {
        return (c1.Position == c2.Position && c1.GameObject == c2.GameObject);
    }

    public static bool operator !=(Cell c1, Cell c2)
    {
        return !(c1 == c2);
    }
}
    
public class GridManager : MonoBehaviour
{
    public Vector2Int GridSize;
    
    //public Dictionary<Vector2Int, Cell> ClosedNodes;
    //public Dictionary<Vector2Int, Cell> OpenNodes;

    public Dictionary<Vector2Int, Cell> AllNodes;
    
    public void InitializeGrid()
    {
        //ClosedNodes = new Dictionary<Vector2Int, Cell>();
        //OpenNodes = new Dictionary<Vector2Int, Cell>();
        AllNodes = new Dictionary<Vector2Int, Cell>();
        
        for (int x = 0; x < GridSize.x; x++)
        for (int y = 0; y < GridSize.y; y++)
        {
            Vector2Int gridPosition = new(x, y);
            AllNodes.Add(gridPosition, new Cell(gridPosition));
        }
    }

    public void SetNode(Cell cell)
    {
        if(AllNodes.ContainsKey(cell.Position))
            AllNodes[cell.Position] = cell;
        //this.OpenNodes.Remove(cell.Position);
        //this.ClosedNodes.Add(cell.Position, cell);
    }

    /*public bool TryOpenNode(Cell cell)
    {
        if (!this.ClosedNodes.Remove(cell.Position)) return false;
        
        this.OpenNodes.Add(cell.Position, cell);
        return true;
    }*/

    public Cell GetCell(Vector2Int gridPosition)
    {
        return AllNodes.GetValueOrDefault(gridPosition);

        /*if (ClosedNodes.TryGetValue(gridPosition, out Cell closedCell)) return closedCell;
        else if (OpenNodes.TryGetValue(gridPosition, out Cell openCell)) return openCell;*/
    }

    public List<Cell> GetNeighbors(Vector2Int gridPosition)
    {
        List<Cell> result = new ();
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                Vector2Int cellPosition = new(gridPosition.x + x, gridPosition.y + y);
                
                if (cellPosition.x < 0 || cellPosition.x >= GridSize.x || 
                    cellPosition.y < 0 || cellPosition.y >= GridSize.y || 
                    Mathf.Abs(x) == Mathf.Abs(y)) {
                    continue;
                }
                
                Cell canditateCell = GetCell(cellPosition);
                if(!canditateCell.IsOccupied) 
                    result.Add(canditateCell);
            }
        }
        return result;
    }
    
}