using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

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
        
        //CreateExtentionRoom(partition);
        
        DrawDoors(partition);
        
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
        
        Rect roomArea = new(partitionArea.x + roomX, partitionArea.y + roomY, roomWidth, roomHeight);
        
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
                cell.Type = CellType.WALL;
            }
            
            partition.Cells.Add(cell);
            this.gridManager.SetNode(cell);
        }
    }
    
    private void CreateExtentionRoom(Partition partition)
    {
        List<Cell> wallCells = partition.GetCellsOfType(CellType.GROUND);
        Cell randomCell = wallCells[Random.Range(0, wallCells.Count)];
        
        var extentionRect = new Rect(randomCell.Position.x - 5/2, randomCell.Position.y - 7/2, 5, 7);
        
        for (int x = (int)extentionRect.x; x < extentionRect.xMax; x++)
        for (int y = (int)extentionRect.y; y < extentionRect.yMax; y++)
        {
            Vector2Int position = new (x, y);

            Cell currentCell = this.gridManager.GetCell(position);
            if (partition.Cells.Contains(currentCell))
            {
                if (x == extentionRect.xMax - 1 || x == extentionRect.x || y == extentionRect.yMax - 1 || y == extentionRect.y)
                {
                    if(currentCell.Type == CellType.WALL)
                        currentCell.GameObject.GetComponent<SpriteRenderer>().color = Color.black;
                }
                else if (currentCell.Type == CellType.WALL)
                {
                    currentCell.GameObject.GetComponent<SpriteRenderer>().color = Color.red;
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
                
                if (x == extentionRect.xMax - 1 || x == extentionRect.x || y == extentionRect.yMax - 1 || y == extentionRect.y)
                {
                    currentCell.GameObject.GetComponent<SpriteRenderer>().color = Color.black;
                    currentCell.Type = CellType.WALL;
                }
                
                partition.Cells.Add(currentCell);
            }
            
            this.gridManager.SetNode(currentCell);
        }
    }

    private void DrawDoors(Partition partition) //TODO separate draw and create
    {
        List<Cell> wallCells = partition.GetCellsOfType(CellType.WALL);

        int doorAmount = 1;//Random.Range(0, 100) < 75 ? 1 : 2;
        while (doorAmount > 0)
        {
            Cell wallCell = wallCells[Random.Range(0, wallCells.Count)];
            
            //Check for empty space
            List<Cell> neighbors = this.gridManager.GetNeighbors(wallCell.Position);
            bool hasConnectedOOutside = neighbors.Count < 4;
            
            bool hasConnectedGround = false;
            foreach (var neighbor in neighbors)
            {
                if(neighbor.Type == CellType.GROUND) hasConnectedGround = true;
            }
            if(!hasConnectedGround || !hasConnectedOOutside) continue;
            
            wallCell.GameObject.GetComponent<SpriteRenderer>().sprite = doorSprite;
            wallCell.Type = CellType.DOOR;
            wallCell.IsOccupied = false;
            
            doorAmount--;
        }
    }

    /*private void DrawHallways() //TODO separate draw and create
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
    }*/
}
