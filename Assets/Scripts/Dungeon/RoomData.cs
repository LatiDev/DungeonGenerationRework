using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RoomData
{
    public enum RoomType
    {
        Basic,
        Start,        
        Platform,
        End
    }

    public Vector3Int Position;
    public RoomType Type;

    public int WallActivated;
    public bool IsActive;

    public void Configure(Vector3Int p, RoomType t)
    {
        this.Position = p;
        this.Type = t;

        this.IsActive = true;
    }
    public void SetupWalls(int rls)
    {
        this.WallActivated = rls;
    }

}
