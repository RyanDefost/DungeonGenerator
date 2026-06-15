using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

public class Generator : MonoBehaviour
{
    [SerializeField] private Vector2Int boardSize;
    [Range(10,90), SerializeField] private int minRoomSize;
    [Range(10,90), SerializeField] private int maxRoomSize;
    
    [Space, SerializeField] private GameObject tilePrefab;
    
    private GameObject[,] boardPositionsFloor;

    private List<Rect> rooms = new();
    
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
        
        if (partition.PartitionArea.width > maxRoomSize || partition.PartitionArea.height > maxRoomSize || Random.Range(0.0f,1.0f) > 0.25)
        {
            if (partition.Split(minRoomSize, maxRoomSize))
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
                
        rooms.Add(new Rect (partitionArea.x + roomX, partitionArea.y + roomY, roomWidth, roomHeight));
    }
    
    private void DrawRooms()
    {
        foreach (Rect room in rooms)
        {
            Color newColor = Random.ColorHSV();
            for (int x = (int)room.x; x < room.xMax; x++)
            for (int y = (int)room.y; y < room.yMax; y++)
            {
                GameObject instance = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                instance.GetComponent<SpriteRenderer>().color = newColor;
                instance.transform.SetParent(transform);
                boardPositionsFloor[x,y] = instance;
            }
        }
    }
}
