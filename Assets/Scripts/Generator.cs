using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;


public class Generator : MonoBehaviour
{
    [SerializeField] public Vector2Int boardSize;
    [SerializeField] public Vector2Int roomSize;
    [SerializeField] private GameObject tilePrefab;
    
    private GameObject[,] boardPositionsFloor;
    
    public void Generate()
    {
        ClearRooms();
        
        SubDungeon rootDungeon = new (new Rect(0, 0, boardSize.x, boardSize.y));
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
    
    public void CreateBSP(SubDungeon subDungeon)
    {
        if (subDungeon.IsLeaf())
        {
            if (subDungeon.Rect.width > roomSize.y 
                || subDungeon.Rect.height > roomSize.y
                || Random.Range(0.0f,1.0f) > 0.25)
            {
                if (subDungeon.Split(roomSize.x, roomSize.y))
                {
                    CreateBSP(subDungeon.leftDungeon);
                    CreateBSP(subDungeon.rightDungeon);
                }
            }
        }
    }

    public void DrawRooms(SubDungeon subDungeon)
    {
        if(subDungeon == null) return;

        if (subDungeon.IsLeaf())
        {
            for (int x = (int)subDungeon.Room.x; x < subDungeon.Room.xMax; x++)
            {
                for (int y = (int)subDungeon.Room.y; y < subDungeon.Room.yMax; y++)
                {
                    GameObject instance = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(transform);
                    boardPositionsFloor[x,y] = instance;
                }
            }
        }
        else
        {
            DrawRooms(subDungeon.leftDungeon);
            DrawRooms(subDungeon.rightDungeon);
        }
    }
}

public class SubDungeon
{
    public SubDungeon leftDungeon, rightDungeon;
    public Rect Rect;
    public Rect Room = new(-1, -1, 0, 0);
    public int debugId = 0;
    
    private static int debugCounter = 0;
    
    public SubDungeon(Rect rect)
    {
        this.Rect = rect;
        debugId = debugCounter;
        debugCounter++;
    }

    public bool IsLeaf() => leftDungeon == null && rightDungeon == null;

    public bool Split(int minRoomSize, int maxRoomSize)
    {
        if(!IsLeaf()) return false;

        bool splitHorizontal;
        if (Rect.width / Rect.height >= 1.25) splitHorizontal = false;
        else if (Rect.height / Rect.width >= 1.25) splitHorizontal = true;
        else splitHorizontal = Random.Range(0.0f, 1.0f) > 0.5f;

        if (Mathf.Min(Rect.height, Rect.width) / 2 < minRoomSize)
        {
            return false;
        }

        if (splitHorizontal)
        {
            int split =  Random.Range(minRoomSize, (int)(Rect.width - minRoomSize));
            
            leftDungeon = new SubDungeon(new Rect(Rect.x, Rect.y, Rect.width, split));
            rightDungeon = new SubDungeon(new Rect(Rect.x, Rect.y + split, Rect.width, Rect.height - split));
        }
        else
        {
            int split =  Random.Range(minRoomSize, (int)(Rect.height - minRoomSize));
            
            leftDungeon = new SubDungeon(new Rect(Rect.x, Rect.y, split, Rect.height));
            rightDungeon = new SubDungeon(new Rect(Rect.x + split, Rect.y, Rect.width - split, Rect.height));
        }
        
        return true;
    }
    
    public void CreateRoom()
    {
        leftDungeon?.CreateRoom();
        rightDungeon?.CreateRoom();

        if (IsLeaf())
        {
            int roomWidth = (int)Random.Range(Rect.width / 2, Rect.width - 2);
            int roomHeight = (int)Random.Range(Rect.height / 2, Rect.height - 2);
            int RoomX = (int)Random.Range(1, Rect.width - roomWidth - 1);
            int RoomY = (int)Random.Range(1, Rect.height - roomHeight - 1);
            
            this.Room = new Rect (this.Rect.x + RoomX, this.Rect.y + RoomY, roomWidth, roomHeight);
        }
    }
}
