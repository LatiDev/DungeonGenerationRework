using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerationResult
{
    public RoomData StarterRoom;
    public RoomData EndRoom;

    public MapGenerationResult(RoomData sr, RoomData er)
    {
        this.StarterRoom = sr;
        this.EndRoom = er;
    }
}
