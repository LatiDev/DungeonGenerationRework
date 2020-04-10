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
    [SerializeField] private GameObject StaircasePrefab;
    [SerializeField] private GameObject StarterPrefab;
    [SerializeField] private GameObject EndPrefab;

    [SerializeField] private Transform RoomParent;

    private MapGenerator MG = new MapGenerator();

    public void CreateLayer()
    {
        Transform cl = CreateLayer(0);

        foreach (RoomData rd in MG.Generate(new Vector3Int(ScaleX, ScaleY, ScaleZ), RoomNumber))
        {
            CreateRoom(rd, cl);
        }
    }
    private Transform CreateLayer(int n)
    {
        GameObject lo = new GameObject();

        lo.name = $"Layer {n}";
        lo.transform.SetParent(RoomParent);

        return lo.transform;
    }
    private void CreateRoom(GameObject g, Transform t, Vector3Int p, List<RelativePosition> rp)
    {
        GameObject r = Instantiate(g);
        r.transform.position = p * 10;
        r.transform.SetParent(t);

        Mapable m = r.GetComponent<Mapable>();
        m.Position = p;

        Room room = r.GetComponent<Room>();
        room?.SetWall(rp);                       
    }
    private void CreateRoom(RoomData rd, Transform p)
    {
        if (rd.Type == RoomData.RoomType.Platform)
        {
            CreateRoom(StaircasePrefab, p, rd.Position, rd.WallActivated);
        }
        else if (rd.Type == RoomData.RoomType.Start)
        {
            CreateRoom(StarterPrefab, p, rd.Position, rd.WallActivated);
        }
        else if (rd.Type == RoomData.RoomType.End)
        {
            CreateRoom(EndPrefab, p, rd.Position, rd.WallActivated);
        }
        else
        {
            CreateRoom(BasisRoomPrefab, p, rd.Position, rd.WallActivated);
        }
    }
}
