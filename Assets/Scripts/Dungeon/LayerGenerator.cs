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

    private GenerateMapable GM = new GenerateMapable();
    
    private void Update()
    {
        foreach(Mapable_Info[,] l in GM.Generate(ScaleX, ScaleY, RoomNumber, Layer))
        {
            //print(l.Length);
        }   
    }
}
