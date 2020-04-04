using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class LayerGenerator : MonoBehaviour
{
    [SerializeField] [Range(10, 999)] private int ScaleX = 100;
    [SerializeField] [Range(10, 999)] private int ScaleY = 100;
    [SerializeField] [Range(10, 999)] private int RoomNumber = 100;
    [SerializeField] [Range(1, 100)] private int Layer = 1;

    [SerializeField] private GameObject RoomPrefab;
    [SerializeField] private GameObject StaircasePrefab;

    private GenerateMapable GM = new GenerateMapable();
    
    private void Update()
    {
        foreach(GenerationData g in GM.Generate(ScaleX, ScaleY, RoomNumber, Layer))
        {
            Create(g.Layer, ScaleX, ScaleY);
        }   
    }

    public void Create(Mapable_Info[,] l, int sx, int sy)
    {
        for (int x = 0; x < sx; x++) 
        { 
            for (int y = 0; y < sy; y++)
            {
                Mapable_Info Mapable = l[x, y];
                
                if (Mapable != null)
                {
                    if (Mapable.IsPlatform)
                    {
                        GameObject r = Instantiate(StaircasePrefab);
                        r.transform.position = new Vector3(Mapable.Map_X, Mapable.Map_Z, Mapable.Map_Y) * 10;

                        Mapable m = r.GetComponent<Mapable>();
                        m.Position = new Vector2(Mapable.Map_X, Mapable.Map_Y);
                    
                    }
                    else
                    {
                        GameObject r = Instantiate(RoomPrefab);
                        r.transform.position = new Vector3(Mapable.Map_X, Mapable.Map_Z, Mapable.Map_Y) * 10;

                        Mapable m = r.GetComponent<Mapable>();
                        m.Position = new Vector2(Mapable.Map_X, Mapable.Map_Y);
                    }                                       
                }
            }
        }
    }
}
