using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

public class Generator : MonoBehaviour
{
    [SerializeField] private Vector2Int boardSize;
    [SerializeField] private int minRoomSize;
    [SerializeField] private int maxRoomSize;
    
    [SerializeField] private GameObject tilePrefab;
    
    private GameObject[,] boardPositionsFloor;
    
    public void Generate()
    {
        ClearRooms();
        
        Partition rootDungeon = new (new Rect(0, 0, boardSize.x, boardSize.y));
        CreateBSP(rootDungeon);
        rootDungeon.CreateRoom();
        
        boardPositionsFloor = new GameObject[boardSize.x, boardSize.y];
        DrawRooms(rootDungeon);
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

    private void DrawRooms(Partition partition)
    {
        if(partition == null) return;

        if (partition.IsLeaf())
        {
            for (int x = (int)partition.Room.x; x < partition.Room.xMax; x++)
            for (int y = (int)partition.Room.y; y < partition.Room.yMax; y++)
            {
                GameObject instance = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                instance.transform.SetParent(transform);
                boardPositionsFloor[x,y] = instance;
            }
        }
        else
        {
            DrawRooms(partition.LeftPartition);
            DrawRooms(partition.RightPartition);
        }
    }
}
