using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

public class Generator : MonoBehaviour
{
    [Range(6,25), SerializeField] private int maxPartitionSize = 1;
    [Range(6,25), SerializeField] private int minPartitionSize = 1;
    [Space]
    [SerializeField] private int minRoomWith = 2;
    [SerializeField] private int minRoomHeight = 2;
    [Range(1, 2), SerializeField] private float sizeOffset = 1;
    
    [Space, SerializeField] private GameObject tilePrefab;
    [Space, SerializeField] private GameObject hallwayPrefab;
    [Space, SerializeField] private Sprite doorSprite;
    
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Astar astar;

    [SerializeField] private bool displayPartitions = true;
    
    private readonly List<Partition> partitions = new();
    
    public void Generate()
    {
        ClearRooms();
        this.gridManager.InitializeGrid();
        
        Partition rootDungeon = new (new Rect(0, 0, this.gridManager.GridSize.x, this.gridManager.GridSize.y));
        
        CreateBSP(rootDungeon);
        
        partitions.Clear();
        InitializeRooms(rootDungeon);
        
        Partition startPartition = partitions[Random.Range(0, partitions.Count)];
        startPartition.isConnected = true;
        ConnectRooms(startPartition);
        HallwayWalls();
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
        
        if(displayPartitions) DisplayPartition(partition);
        
        if(!CreateBaseRoom(partition)) return;
        CreateExtentionRoom(partition);
        DrawDoors(partition);
        
        partitions.Add(partition);
    }

    public void DisplayPartition(Partition partition)
    {
        
        Color color = Random.ColorHSV();
        for (int x = (int)partition.PartitionArea.x; x < partition.PartitionArea.xMax; x++)
        for (int y = (int)partition.PartitionArea.y; y < partition.PartitionArea.yMax; y++)
        {
            GameObject instance = Instantiate(tilePrefab, new Vector3(x, y, -1), Quaternion.identity);
            instance.GetComponent<SpriteRenderer>().color = color;
            instance.transform.SetParent(transform);
        }
    }
    
    private bool CreateBaseRoom(Partition partition)
    {
        Rect partitionArea = partition.PartitionArea;
        float roomWidth = (int)Random.Range(partitionArea.width / sizeOffset, partitionArea.width - 2);
        float roomHeight = (int)Random.Range(partitionArea.height / sizeOffset, partitionArea.height - 2);
        int roomX = (int)Random.Range(1, partitionArea.width - roomWidth - 1);
        int roomY = (int)Random.Range(1, partitionArea.height - roomHeight - 1);
        
        Rect roomArea = new(partitionArea.x + roomX, partitionArea.y + roomY, roomWidth, roomHeight);
        
        if(roomArea.width < minRoomWith || roomArea.height < minRoomHeight) return false;
        
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
                instance.GetComponent<SpriteRenderer>().color = Color.gray1;
                cell.Type = CellType.WALL;
            }
            
            partition.Cells.Add(cell);
            this.gridManager.SetNode(cell);
        }

        return true;
    }
    
    private void CreateExtentionRoom(Partition partition)
    {
        List<Cell> wallCells = partition.GetCellsOfType(CellType.GROUND);
        if(wallCells.Count == 0) return;
        
        if(Random.Range(0, 100) < 25) return;
        
        Cell centerCell = wallCells[Random.Range(0, wallCells.Count)];
        Rect extentionRect = new(centerCell.Position.x - 5/2, centerCell.Position.y - 7/2, 5, 7);

        foreach (var part in partitions)
        {
            if (part == partition) continue;
            if (extentionRect.Overlaps(part.PartitionArea)) return;
        }
        
        for (int x = (int)extentionRect.x; x < extentionRect.xMax; x++)
        for (int y = (int)extentionRect.y; y < extentionRect.yMax; y++)
        {
            Cell currentCell = this.gridManager.GetCell(new (x, y)) ?? this.gridManager.SetNode(new Cell(new Vector2Int(x,y)));

            if(currentCell.Type != CellType.NONE)
            {
                if (x == extentionRect.xMax - 1 || x == extentionRect.x || y == extentionRect.yMax - 1 || y == extentionRect.y)
                {
                    if(currentCell.Type == CellType.WALL)
                        currentCell.GameObject.GetComponent<SpriteRenderer>().color = Color.gray1;
                }
                else if (currentCell.Type == CellType.WALL)
                {
                    currentCell.GameObject.GetComponent<SpriteRenderer>().color = Color.white;
                    currentCell.Type = CellType.GROUND;
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
                partition.Cells.Add(currentCell);
                
                
                if (x == extentionRect.xMax - 1 || x == extentionRect.x || y == extentionRect.yMax - 1 || y == extentionRect.y)
                {
                    currentCell.GameObject.GetComponent<SpriteRenderer>().color = Color.gray1;
                    currentCell.Type = CellType.WALL;
                }
            }
            
            this.gridManager.SetNode(currentCell);
        }
    }

    private void DrawDoors(Partition partition) //TODO separate draw and create
    {
        List<Cell> wallCells = partition.GetCellsOfType(CellType.WALL);
        if(wallCells.Count == 0) return;
        
        int doorAmount = 1;// Random.Range(0, 100) < 75 ? 1 : 2;
        int maxLoops = 10;
        while (doorAmount > 0)
        {
            Cell wallCell = wallCells[Random.Range(0, wallCells.Count)];
            
            //Check for empty space
            List<Cell> neighbors = this.gridManager.GetNeighbors(wallCell.Position);
            
            bool hasConnectedGround = false;
            bool hasConnectedOOutside = false;
            foreach (var neighbor in neighbors)
            {
                if(neighbor.Type == CellType.GROUND) hasConnectedGround = true;
                if(neighbor.Type == CellType.NONE) hasConnectedOOutside = true;
            }
            maxLoops--;
            if (maxLoops == 0) //TODO IF NEVER THE CASE REMOVE
            {
                Debug.LogWarning("inf-Loop");
                break;
            }
            if(!hasConnectedGround || !hasConnectedOOutside) continue;
            
            wallCell.GameObject.GetComponent<SpriteRenderer>().color = Color.wheat;
            wallCell.Type = CellType.DOOR;
            wallCell.IsOccupied = false;
            
            doorAmount--;
        }
    }

    //Grab random start partition
    // Foreach door
        // Get the closest partition from door == not self / != connected
        // get path to the closest door
        // If reached hallway or Door => STOP and set connected.
    // Set next 
    
    private void ConnectRooms(Partition partition) //TODO separate draw and create
    {
        //Get partition door
        Cell partitionDoor = partition.GetCellsOfType(CellType.DOOR).First();
        if (partitionDoor == null)
        {
            Debug.LogWarning($"{partition} does not have door");
            return;
        }
        
        //Get closest non connected partition
        float closestDistance = 999;
        Partition closestPartition = null;
        foreach (Partition otherPartition in partitions)
        {
            if(partition == otherPartition || otherPartition.isConnected) continue;
                
            float distance = Vector2.Distance(partitionDoor.Position, otherPartition.PartitionArea.center);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPartition = otherPartition;
            }
        }

        if (closestPartition == null)
        {
            Debug.LogWarning($"{partition} can not connect to antoher partition");
            return;
        }
        
        
        //Get partition door
        Cell otherDoor = closestPartition.GetCellsOfType(CellType.DOOR).First();
        if (otherDoor == null)
        {
            Debug.LogWarning($"{partition} does not have door");
            return;
        }
        
        //Get path from door to otherPartition door
        List<Vector2Int> hallwayCells = astar.FindPathToTarget(otherDoor.Position, partitionDoor.Position);
        
        //Draw path
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
            partition.Cells.Add(cell);
        }
        
        //Set connected true
        closestPartition.isConnected = true;
        //ConnectRooms(otherPartition)
        ConnectRooms(closestPartition);
    }

    private void HallwayWalls()
    {
        foreach (Partition partition in partitions)
        {
            List<Cell> hallwayTiles = partition.GetCellsOfType(CellType.HALLWAY);

            foreach (var cell in hallwayTiles)
            {
                foreach (Cell neighbor in this.gridManager.GetNeighbors(cell.Position))
                {
                    if (neighbor.Type == CellType.NONE)
                    {
                        GameObject instance = Instantiate(tilePrefab, new Vector3(neighbor.Position.x, neighbor.Position.y, 0), Quaternion.identity);
                        instance.gameObject.GetComponent<SpriteRenderer>().color = Color.gray1;
                        instance.transform.SetParent(transform);
                        
                        Cell wall = new(
                            neighbor.Position,
                            CellType.HALLWAY,
                            instance,
                            true
                        );
                        
                        this.gridManager.SetNode(wall);
                    }
                }
            }
        }
    }
}
