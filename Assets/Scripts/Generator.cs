using System.Collections.Generic;
using Partitions;
using Passes.DataPasses;
using Passes.RoomPasses;
using Random = UnityEngine.Random;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [HideInInspector] public int Seed = 404;
    
    [Space, Header("Settings")]
    [Range(6,25), SerializeField] private int maxPartitionSize = 1;
    [Range(6,25), SerializeField] private int minPartitionSize = 1;
    [Space]
    public int minRoomWith = 2;
    public int minRoomHeight = 2;
    [Range(1, 2)] public float sizeOffset = 1;
    
    [Header("Debug")]
    [SerializeField] private bool displayPartitions = true;
    [SerializeField] private bool displayStartDistance = true;
    
    [Header("Refs")]
    public GridManager gridManager;
    
    public readonly List<Partition> partitions = new();
    
    //Passes
    private VisualPartitionsPass visualPartitions;
    private RoomBasePass roomBasePass;
    private RoomExtensionPass roomExtensionPass;
    private RoomDoorPass roomDoorPass;
    private HallwayPass hallwayPass;
    private RoomDistancePass roomDistancePass;
    private RoomColorFillPass roomColorFillPass;
    private RoomTypePass roomTypePass;
    
    public void Generate(int seed)
    {
        Random.InitState(seed);
        
        this.visualPartitions  ??= new VisualPartitionsPass(this);
        this.roomBasePass      ??= new RoomBasePass(this);
        this.roomExtensionPass ??= new RoomExtensionPass(this);
        this.roomDoorPass      ??= new RoomDoorPass(this);
        this.hallwayPass       ??= new HallwayPass(this);
        this.roomDistancePass  ??= new RoomDistancePass(this);
        this.roomColorFillPass ??= new RoomColorFillPass(this);
        this.roomTypePass      ??= new RoomTypePass(this);
        
        this.partitions.Clear();
        ClearRooms();
        
        this.gridManager.InitializeGrid();
        
        Partition rootDungeon = new (
            new Rect(0, 0, this.gridManager.GridSize.x, this.gridManager.GridSize.y)
        );
        
        CreateBSP(rootDungeon);
        GenerateRoom(rootDungeon);
        GenerateRoomPasses();
        
    }
    
    private void CreateBSP(Partition partition)
    {
        if (!partition.IsLeaf()) return;
        
        if (!(partition.PartitionArea.width > maxPartitionSize) 
            && !(partition.PartitionArea.height > maxPartitionSize)) return;
        
        
        if (partition.Split(minPartitionSize, maxPartitionSize))
        {
            CreateBSP(partition.LeftPartition);
            CreateBSP(partition.RightPartition);
        }
    }

    private void GenerateRoom(Partition partition)
    {
        if(partition.LeftPartition != null) GenerateRoom(partition.LeftPartition);
        if(partition.RightPartition != null) GenerateRoom(partition.RightPartition);

        if (!partition.IsLeaf()) return;
        
        if(displayPartitions) //DEBUG
            this.visualPartitions.DisplayPartition(partition);

        if (roomBasePass.CreateBaseRoom(partition))
        {
            this.roomExtensionPass.CreateExtensionRoom(partition);
            this.roomDoorPass.DrawDoors(partition);
            this.partitions.Add(partition);
        }
    }

    private void GenerateRoomPasses()
    {
        Partition startRoom = partitions[Random.Range(0, partitions.Count)];
        
        hallwayPass.ConnectRooms(startRoom);
        hallwayPass.HallwayWalls();
        
        // SET DATA
        
        if(displayStartDistance) 
            this.roomColorFillPass.floodColor(startRoom);
        
        this.roomDistancePass.AssignDistance(startRoom);
        this.roomTypePass.AssignPartitionType();
    }
    
    public void ClearRooms()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if(child.gameObject == this.gameObject) continue;
            DestroyImmediate(child.gameObject);
        }
    }
}
