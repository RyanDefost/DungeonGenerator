using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

public struct Room
{
    public Rect roomArea;
    public Vector2Int Door;
    
    public Room(Rect roomArea, Vector2Int door) 
    {
        this.roomArea = roomArea;
        this.Door  = door;
    }
}

public class Generator : MonoBehaviour
{
    [SerializeField] private Vector2Int boardSize;
    [Range(1,25), SerializeField] private int minPartitionSize = 1;
    [Range(1,25), SerializeField] private int maxPartitionSize = 1;
    
    [Space, SerializeField] private GameObject tilePrefab;
    [Space, SerializeField] private GameObject doorPrefab;
    
    private GameObject[,] boardPositionsFloor;

    private readonly List<Room> rooms = new();
    
    public void Generate()
    {
        ClearRooms();
        rooms.Clear();
        
        Partition rootDungeon = new (new Rect(0, 0, boardSize.x, boardSize.y));
        CreateBSP(rootDungeon);
        CreateRoom(rootDungeon);
        
        boardPositionsFloor = new GameObject[boardSize.x, boardSize.y];
        
        DrawRooms();
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
        
        if (partition.PartitionArea.width > maxPartitionSize || partition.PartitionArea.height > maxPartitionSize || Random.Range(0.0f,1.0f) > 0.25)
        {
            if (partition.Split(minPartitionSize, maxPartitionSize))
            {
                CreateBSP(partition.LeftPartition);
                CreateBSP(partition.RightPartition);
            }
        }
    }

    private void CreateRoom(Partition partition)
    {
        if(partition.LeftPartition != null) CreateRoom(partition.LeftPartition);
        if(partition.RightPartition != null) CreateRoom(partition.RightPartition);

        if (!partition.IsLeaf()) return;
        
        Rect partitionArea = partition.PartitionArea;
        int roomWidth = (int)Random.Range(partitionArea.width / 2, partitionArea.width - 2);
        int roomHeight = (int)Random.Range(partitionArea.height / 2, partitionArea.height - 2);
        int roomX = (int)Random.Range(1, partitionArea.width - roomWidth - 1);
        int roomY = (int)Random.Range(1, partitionArea.height - roomHeight - 1);
                
        rooms.Add(new Room(
            new Rect (partitionArea.x + roomX, partitionArea.y + roomY, roomWidth, roomHeight),
            new Vector2Int((int)(partitionArea.x + roomX), (int)(partitionArea.y + roomY))
        ));
    }
    
    private void DrawRooms()
    {
        foreach (Room room in rooms)
        {
            Rect roomRect = room.roomArea;

            Color newColor = Color.white;//Random.ColorHSV();
            for (int x = (int)roomRect.x; x < roomRect.xMax; x++)
            for (int y = (int)roomRect.y; y < roomRect.yMax; y++)
            {
                GameObject prefab = room.Door == new Vector2Int(x,y) ? doorPrefab : tilePrefab;
                
                GameObject instance = Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                //instance.GetComponent<SpriteRenderer>().color = newColor;
                instance.transform.SetParent(transform);
                boardPositionsFloor[x,y] = instance;
            }
        }
    }
}
