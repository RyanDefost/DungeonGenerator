using System.Collections.Generic;
using Entities;
using Partitions;
using Passes.DataPasses;
using Passes.DungeonPass;
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
    [SerializeField] private bool showDebug = true;
    
    [Header("Refs")]
    public GridManager gridManager;

    public EntitySettings EnemySettings;
    
    public readonly List<Partition> partitions = new();
    public Partition startPartition { get; private set; }
    
    [Space]
    [SerializeField] private List<BaseRoomPass> roomPasses = new();
    [SerializeField] private List<BaseDungeonPass> generalPasses = new();
    
    public void Generate(int seed)
    {
        Random.InitState(seed);
        
        this.partitions.Clear();
        ClearRooms();
        
        this.gridManager.InitializeGrid();
        
        Partition rootDungeon = new (
            new Rect(0, 0, this.gridManager.GridSize.x, this.gridManager.GridSize.y)
        );
        
        CreateBSP(rootDungeon);
        GenerateRoomPasses(rootDungeon);
        GenerateGeneralPasses();
        
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

    private void GenerateRoomPasses(Partition partition)
    {
        if(partition.LeftPartition != null) GenerateRoomPasses(partition.LeftPartition);
        if(partition.RightPartition != null) GenerateRoomPasses(partition.RightPartition);
        
        if (!partition.IsLeaf()) return;
        
        foreach (BaseRoomPass roomPass in roomPasses)
        {
            if(roomPass.isDebug && !this.showDebug) continue;
            
            roomPass.Connect(this);
            
            bool success = roomPass.SetPass(partition);
            if (roomPass.isRequired && !success)
            {
                Debug.LogWarning($"{roomPass.name} failed to set required pass");
                return;
            }
        }
        this.partitions.Add(partition);
    }

    private void GenerateGeneralPasses()
    {
        startPartition = partitions[Random.Range(0, partitions.Count)];

        foreach (var dungeonPass in generalPasses)
        {
            if(dungeonPass.isDebug && !this.showDebug) continue;
                        
            dungeonPass.Connect(this);
            
            bool success = dungeonPass.SetPass();
            if (dungeonPass.isRequired && !success)
            {
                Debug.LogWarning($"{dungeonPass.name} failed to set required pass");
                return;
            }
        }
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
