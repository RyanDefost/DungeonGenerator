using System.Collections.Generic;
using UnityEngine;
    
public class GridManager : MonoBehaviour
{
    public Vector2Int GridSize;
    public Dictionary<Vector2Int, Cell> AllNodes;
    
    public void InitializeGrid()
    {
        AllNodes = new Dictionary<Vector2Int, Cell>();
        
        for (int x = 0; x <= GridSize.x; x++)
        for (int y = 0; y <= GridSize.y; y++)
        {
            Vector2Int gridPosition = new(x, y);
            AllNodes.Add(gridPosition, new Cell(gridPosition));
        }
    }

    public void SetNode(Cell cell)
    {
        if (AllNodes.ContainsKey(cell.Position))
        {
            AllNodes[cell.Position] = cell;
        }
        else
        {
            Debug.LogWarning($"Cell {cell.Position} added to grid!");
            AllNodes.Add(cell.Position, cell);
        }
    }

    public Cell GetCell(Vector2Int gridPosition)
    {
        return AllNodes.TryGetValue(gridPosition, out Cell cell) ? cell : null;
    }

    public List<Cell> GetNeighbors(Vector2Int gridPosition)
    {
        
        List<Cell> result = new ();
        Vector2Int[] directions = { new(0, 1), new(0, -1), new(1, 0), new(-1, 0) };

        foreach (var direction in directions)
        {
            Cell cell = GetCell(gridPosition + direction);
            if (cell.Type != CellType.NONE) result.Add(cell);
        }

        return result;
        /*for (int x = -1; x < 2; x++)
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
        return result;*/
    }
}