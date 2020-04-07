using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class LayerGenerator : MonoBehaviour
{
    [SerializeField] [Range(10, 999)]   private int ScaleX = 100;
    [SerializeField] [Range(1, 100)]    private int ScaleY = 1;
    [SerializeField] [Range(10, 999)]   private int ScaleZ = 100;

    [SerializeField] [Range(10, 999)] private int RoomNumber = 100;

    [SerializeField] private GameObject RoomPrefab;
    [SerializeField] private GameObject StaircasePrefab;
    [SerializeField] private Transform RoomParent;

    private MapGenerator MG = new MapGenerator();
    
    private void Update()
    {
        foreach(LayerData g in MG.Generate(new Vector3Int(ScaleX, ScaleY, ScaleZ), RoomNumber))
        {
            CreateLayer(g.Layer, ScaleX, ScaleZ, g.LastRoom.Position.y);
        }   
    }
    public Transform CreateLayer(int n)
    {
        GameObject lo = new GameObject();

        lo.name = $"Layer {n}";
        lo.transform.SetParent(RoomParent);

        return lo.transform;
    }
    public void CreateRoom(GameObject g, Transform t, Vector3Int p)
    {
        GameObject r = Instantiate(g);
        r.transform.position = p * 10;
        r.transform.SetParent(t);

        Mapable m = r.GetComponent<Mapable>();
        m.Position = p;
    }
    public void CreateLayer(RoomData[,] l, int sx, int sz, int layernumber)
    {
        Transform Layer = CreateLayer(layernumber);
        
        for (int x = 0; x < sx; x++) 
        { 
            for (int z = 0; z < sz; z++)
            {
                RoomData rd = l[x, z];
                
                if (rd.IsActive == true)
                {
                    if (rd.Type == RoomData.RoomType.Platform)
                    {
                        CreateRoom(StaircasePrefab, Layer, rd.Position);                  
                    }
                    else
                    {
                        CreateRoom(RoomPrefab, Layer, rd.Position);
                    }                                       
                }
            }
        }
    }
}
