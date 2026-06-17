using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

public enum RoomType
{
    NONE = 0,
    LOOT,
    START,
    END
}

public class Room
{
    public Rect roomArea;

    public int doorAmount = Random.Range(0, 100) < 75 ? 1 : 2;
    public List<Vector2Int> doorPositions;

    public RoomType RoomType = RoomType.NONE;
    
    public Room neighbor;

    public Room(Rect roomArea) 
    {
        this.roomArea = roomArea;
        this.doorPositions = GenerateDoorPositions(roomArea, this.doorAmount);
    }
    
    private static List<Vector2Int> GenerateDoorPositions(Rect area, int amount)
    {
        int amountLeft = amount;
        List<Vector2Int> positions = new();
        
        while (amountLeft > 0)
        {
            bool isHorizontal = Random.Range(0, 100) < 40;
            bool isTop = Random.Range(0, 100) < 40;

            float xPos, yPos;
            if (isHorizontal)
            {
                xPos = (area.xMax - 1) - Random.Range(1, area.width - 2);
                yPos = isTop ? area.yMax - 1 : area.yMin;
            }
            else
            {
                yPos = (area.yMax - 1) - Random.Range(1, area.height - 2);
                xPos = isTop ? area.xMax - 1 : area.xMin;
            }

            Vector2Int pos = new (Mathf.CeilToInt(xPos), Mathf.CeilToInt(yPos));
            if(positions.Contains(pos)) continue;
            
            positions.Add(pos);
            amountLeft--;
        }
        
        return positions;
    }
}

public class Generator : MonoBehaviour
{
    [Range(1,25), SerializeField] private int maxPartitionSize = 1;
    [Range(1,25), SerializeField] private int minPartitionSize = 1;
    [Space]
    [SerializeField] private int maxRoomWith = 5;
    [SerializeField] private int minRoomWith = 2;
    [Space]
    [SerializeField] private int maxRoomHeight = 5;
    [SerializeField] private int minRoomHeight = 2;
    
    [Space, SerializeField] private GameObject tilePrefab;
    [Space, SerializeField] private GameObject hallwayPrefab;
    [Space, SerializeField] private Sprite doorSprite;
    
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Astar astar;

    private readonly List<Partition> partitions = new();
    
    public void Generate()
    {
        ClearRooms();
        this.gridManager.InitializeGrid();
        
        Partition rootDungeon = new (new Rect(0, 0, this.gridManager.GridSize.x, this.gridManager.GridSize.y));
        
        CreateBSP(rootDungeon);
        
        partitions.Clear();
        InitializeRooms(rootDungeon);
        
        
        //DrawHallways();
    }

    public void ClearRooms()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if(child.gameObject == this.gameObject) continue;
            DestroyImmediate(child.gameObject);
        }
    }
    
    private void CreateBSP(Partition partition)
    {
        if (!partition.IsLeaf()) return;
        
        if (partition.PartitionArea.width > maxPartitionSize || partition.PartitionArea.height > maxPartitionSize)
        {
            if (partition.Split(minPartitionSize, maxPartitionSize))
            {
                CreateBSP(partition.LeftPartition);
                CreateBSP(partition.RightPartition);
            }
        }
    }

    private void InitializeRooms(Partition partition)
    {
        if(partition.LeftPartition != null) InitializeRooms(partition.LeftPartition);
        if(partition.RightPartition != null) InitializeRooms(partition.RightPartition);

        if (!partition.IsLeaf()) return;
        
        CreateBaseRoom(partition);
        DrawRoom(partition);
        
        CreateExtentionRoom(partition);
        
        //DrawDoors(partition);
        
        partitions.Add(partition);
    }

    private void CreateBaseRoom(Partition partition)
    {
        Rect partitionArea = partition.PartitionArea;
        float roomWidth = (int)Random.Range(partitionArea.width / 2, partitionArea.width - 2);
        float roomHeight = (int)Random.Range(partitionArea.height / 2, partitionArea.height - 2);
        int roomX = (int)Random.Range(1, partitionArea.width - roomWidth - 1);
        int roomY = (int)Random.Range(1, partitionArea.height - roomHeight - 1);
        
        roomWidth = Mathf.Clamp(roomWidth, minRoomWith, maxRoomWith);
        roomHeight = Mathf.Clamp(roomHeight, minRoomHeight, maxRoomHeight);
        
        Rect roomRect = new(partitionArea.x + roomX, partitionArea.y + roomY, roomWidth, roomHeight);
        partition.room = new Room(roomRect);
    }
    /// <summary>
    /// THE PARTITION SHOULD HAVE A LIST OF ALL THE CELLS, THIS INSTEAD OF THE ROOM WILL CONTROLL EVERYTHING
    /// FIRST GENERATE CELLS BASED ON THE ROOM RECT
    /// THEN GENERATE CELLS BASED ON THE EXTENTION
    /// THEN CHECK FOR WALLS AND PLACE A RANDOM DOOR.
    /// </summary>
    /// <param name="partition"></param>
    private void CreateExtentionRoom(Partition partition)
    {
        int pointInRoomX = (int)Random.Range(partition.room.roomArea.x + 2, partition.room.roomArea.xMax - 2);
        int pointInRoomY = (int)Random.Range(partition.room.roomArea.y + 2, partition.room.roomArea.yMax - 2);

        //rect //TODO ISNT CENTRAL
        var extentionRect = new Rect(pointInRoomX - partition.room.roomArea.width / 2, pointInRoomY - partition.room.roomArea.height / 2, 5, 7);
        
        for (int x = (int)extentionRect.x; x < extentionRect.xMax; x++)
        for (int y = (int)extentionRect.y; y < extentionRect.yMax; y++)
        {
            Vector2Int position = new (x, y);
            Cell currentCell;
            
            if (partition.Cells.ContainsKey(position))
            {
                currentCell = partition.Cells[position];
                if(currentCell.Type == CellType.WALL) currentCell.GameObject.GetComponent<SpriteRenderer>().color = Color.red;
                if (x == extentionRect.xMax - 1 || x == extentionRect.x || y == extentionRect.yMax - 1 || y == extentionRect.y)
                {
                    if(currentCell.Type == CellType.WALL)
                        currentCell.GameObject.GetComponent<SpriteRenderer>().color = Color.black;
                }
            }
            else
            {
                GameObject instance = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                instance.transform.SetParent(transform);
                
                currentCell = new Cell(
                    new Vector2Int(x, y),
                    CellType.GROUND,
                    instance,
                    true
                );
                
                if (x == extentionRect.xMax - 1 || x == extentionRect.x || y == extentionRect.yMax - 1 || y == extentionRect.y)
                {
                    currentCell.GameObject.GetComponent<SpriteRenderer>().color = Color.black;
                }
                
                partition.Cells.Add(currentCell.Position, currentCell);
            }
            
            this.gridManager.SetNode(currentCell);
        }
    }

    
    
    private void DrawRoom(Partition partition)
    {
        Rect roomArea = partition.room.roomArea;
        
        for (int x = (int)roomArea.x; x < roomArea.xMax; x++)
        for (int y = (int)roomArea.y; y < roomArea.yMax; y++)
        {
            GameObject instance = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
            instance.transform.SetParent(transform);

            Cell cell = new(
                new Vector2Int(x,y),
                CellType.GROUND,
                instance,
                true
            );
            if (x == roomArea.xMax - 1 || x == roomArea.x || y == roomArea.yMax - 1 || y == roomArea.y)
            {
                instance.GetComponent<SpriteRenderer>().color = Color.black;
                cell.Type =  CellType.WALL;
            }
            this.gridManager.SetNode(cell);
            partition.Cells.Add(cell.Position, cell);

        }
    }

    private void DrawDoors(Partition partition) //TODO separate draw and create
    {
        foreach (Vector2Int doorPosition in partition.room.doorPositions)
        {
            Cell doorCell = this.gridManager.GetCell(doorPosition);
            doorCell.GameObject.GetComponent<SpriteRenderer>().sprite = doorSprite;
            
            doorCell.Type = CellType.DOOR;
            doorCell.IsOccupied = false;
        }
    }

    private void DrawHallways() //TODO separate draw and create
    {
        foreach (Partition partition in partitions)
        {
            float closestDistance = 999;
            foreach (Partition otherPartition in partitions)
            {
                if(partition == otherPartition) continue;
                
                float distance = Vector2.Distance(partition.room.roomArea.center, otherPartition.room.roomArea.center);
                if (distance < closestDistance && otherPartition.room.neighbor != partition.room)
                {
                    partition.room.neighbor = otherPartition.room;
                    closestDistance = distance;
                }
            }

            foreach (Vector2Int doorPosition in partition.room.doorPositions)
            {
                List<Vector2Int> hallwayCells = astar.FindPathToTarget(doorPosition, partition.room.neighbor.doorPositions[0]);

                foreach (Vector2Int cellPosition in hallwayCells)
                {
                    if (this.gridManager.GetCell(cellPosition).Type == CellType.HALLWAY) break;
                    
                    GameObject instance = Instantiate(hallwayPrefab, new Vector3(cellPosition.x, cellPosition.y, 0), Quaternion.identity);
                    instance.transform.SetParent(transform);

                    Cell cell = new(
                        cellPosition,
                        CellType.HALLWAY,
                        instance,
                        false
                    );
                    this.gridManager.SetNode(cell);
                }
            }
        }
    }
}
