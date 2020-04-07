using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LayerData
{
    public int RoomNumber;

    public Vector3Int Scale;       
    public Vector3Int StartPositon;
    public Vector3Int EndPosition;

    public RoomData[,] Layer;    

    public RoomData LastRoom
    {
        get
        {
            return this.Layer[EndPosition.x, EndPosition.y];
        }
        set
        {
            this.Layer[EndPosition.x, EndPosition.y] = value;
        }        
    }
}
