using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class LayerGenerator : MonoBehaviour
{
    [SerializeField] [Range(10, 999)] private int ScaleX = 100;
    [SerializeField] [Range(1, 100)] private int ScaleY = 1;
    [SerializeField] [Range(10, 999)] private int ScaleZ = 100;

    [SerializeField] [Range(10, 999)] private int RoomNumber = 100;

    [Header("Rooms Prefabs")]
    [SerializeField] private GameObject BasisRoomPrefab;
    [SerializeField] private GameObject StarterPrefab;
    [SerializeField] private GameObject StaircasePrefab;
    [SerializeField] private GameObject HallwayPrefab;
    [SerializeField] private GameObject EndPrefab;

    [SerializeField] private Transform RoomParent;

    private MapGenerator MG = new MapGenerator();
    private LayerGenerationResult LGR = new LayerGenerationResult();

    public void GenerateLayer(Vector2Int Size)
    {
        MG.GenerateMap(Size);
        //MG.GenerateMap(new Vector3Int(Size.x, Size.y, Size.y), 100);
    }

    public LayerGenerationResult GenerateLayer()
    {
        Transform cl = CreateLayer(0);
        
        foreach (RoomData rd in MG.Generate(new Vector3Int(ScaleX, ScaleY, ScaleZ), RoomNumber))
        {
            CreateRoom(rd, cl);
        }

        return LGR;
    }
    private Transform CreateLayer(int n)
    {
        GameObject lo = new GameObject();

        lo.name = $"Layer {n}";
        lo.transform.SetParent(RoomParent);

        return lo.transform;
    }
    private GameObject InstantiateRoom(GameObject g, Transform t, Vector3Int p, int rp)
    {
        GameObject r = Instantiate(g);
        r.transform.position = p * 20;
        r.transform.SetParent(t);

        Mapable m = r.GetComponent<Mapable>();
        m.Position = p;

        Room room = r.GetComponent<Room>();
        room?.SetWall(rp);

        return r;
    }
    private void CreateRoom(RoomData rd, Transform p)
    {
        if (rd.Type == RoomData.RoomType.Platform)
        {
            GameObject PlatformInstance = InstantiateRoom(StaircasePrefab, p, rd.Position, rd.WallActivated);
        }
        else if (rd.Type == RoomData.RoomType.Start)
        {
            GameObject StarterInstance = InstantiateRoom(StarterPrefab, p, rd.Position, rd.WallActivated);
            LGR.StarterRoomInstance = StarterInstance;
        }
        else if (rd.Type == RoomData.RoomType.End)
        {
            GameObject EndInstance = InstantiateRoom(EndPrefab, p, rd.Position, rd.WallActivated);
        }
        else
        {
            GameObject BasicRoomInstance = InstantiateRoom(BasisRoomPrefab, p, rd.Position, rd.WallActivated);
        }
    }
}
