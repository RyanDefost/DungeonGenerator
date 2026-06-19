using System.Collections.Generic;
using System.Linq;
using Cells;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Vector2Int GridSize;
    [SerializeField] private List<CellSettings> settings;
    
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

    public Cell SetNode(Cell cell)
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

        return cell;
    }

    public Cell GetCell(Vector2Int gridPosition)
    {
        return AllNodes.TryGetValue(gridPosition, out Cell cell) ? cell : null;
    }

    public List<Cell> GetNeighbors(Vector2Int gridPosition, bool withCorners = false)
    {
        
        List<Cell> result = new ();
        Vector2Int[] directions = { new(0, 1), new(0, -1), new(1, 0), new(-1, 0) };
        Vector2Int[] cornerDirections = { new(1, 1), new(1, -1), new(-1, 1), new(-1, 1) };

        if(withCorners) directions = directions.Concat(cornerDirections).ToArray();
        
        foreach (var direction in directions)
        {
            Cell cell = GetCell(gridPosition + direction);
            if (cell != null) result.Add(cell);
        }

        return result;
    }
    
    public List<Cell> GetCellsOfType(CellType type)
    {
        return AllNodes.Values.Where(cell => cell.Type == type).ToList();
    }

    public Cell InstantiateCell(CellType cellType, Vector2Int position, bool isOccupied = false)
    {
        CellSettings settings = GetCellSetting(cellType);
        
        GameObject instance = Instantiate(
            settings.tilePrefab,
            new Vector3(position.x, position.y, settings.zOffset),
            Quaternion.identity
        );
        
        instance.gameObject.GetComponent<SpriteRenderer>().color = settings.cellColor;
        instance.transform.SetParent(transform);

        Cell cell = new(
            position,
            cellType,
            instance,
            isOccupied
        );
        
        SetNode(cell);
        return GetCell(position);
    }

    public CellSettings GetCellSetting(CellType cellType)
    {
        CellSettings settings = this.settings.FirstOrDefault(cellSetting => cellSetting.cellType.CompareTo(cellType) == 0);
        if (settings == null)
        {
            Debug.LogWarning($"CellType setting {cellType}, could not be found in GridManager");
            return null;
        }
        
        return settings;
    }

    public Cell ChangeCellType(Cell cell, CellType newType)
    {
        CellSettings settings = GetCellSetting(newType);

        cell.GameObject.GetComponent<SpriteRenderer>().color = settings.cellColor;
        ;
        cell.Type = newType;

        return cell;
    }
}