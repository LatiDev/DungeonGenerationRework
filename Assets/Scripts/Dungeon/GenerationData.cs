using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GenerationData
{
    public int ScaleX;
    public int ScaleY;
    
    public int RoomNumber;
        
    public int EndX;
    public int EndY;
    public int EndZ;

    public Mapable_Info[,] Layer;
    public Mapable_Info LastMapable;    
}
