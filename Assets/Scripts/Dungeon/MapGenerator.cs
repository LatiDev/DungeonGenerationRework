using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator
{
    private RoomData[,] CurrentLayer;
    private Vector3Int LayerScale;

    private Vector3Int GetRandomPosition(int x, int z) 
    {
        int Randx = Random.Range(0, x);
        int RandZ = Random.Range(0, z);

        return new Vector3Int(Randx, 0, RandZ);
    }    

    public RoomData[] Generate(Vector3Int Scale, int RoomNumber)
    {
        this.CurrentLayer = new RoomData[Scale.x, Scale.z];
        this.LayerScale = Scale;

        List<RoomData> AllRoom = new List<RoomData>(RoomNumber); 

        Vector3Int StarterPositon3D = GetRandomPosition(this.LayerScale.x, this.LayerScale.z);
        RoomData StarterData = SetRoomAt(StarterPositon3D, GetStarter);
        AllRoom.Add(StarterData);

        Vector3Int LastPosition = StarterData.Position;
        for (int rn = 0; rn < RoomNumber; rn++)
        {
            RoomData room = PlaceRoom(LastPosition);
            LastPosition = room.Position;

            AllRoom.Add(room);
        }

        return AllRoom.ToArray();
    }
    private RoomData PlaceRoom(Vector3Int LastRoomPos)
    {
        List<RelativePosition> _moves = LookAroundARoom(LastRoomPos);

        if (_moves.Count == 0)
        {
            //Layer[LastRoomX, LastRoomY] = GetStairCase(LastRoomX, LastRoomY, LastRoomZ);            
            return SetRoomAt(LastRoomPos, GetBasicRoom);
        }
        else
        {
            RelativePosition Direction = _moves[Random.Range(0, _moves.Count)];

            if (Direction == RelativePosition.North) LastRoomPos.z++;
            if (Direction == RelativePosition.South) LastRoomPos.z--;
            if (Direction == RelativePosition.East) LastRoomPos.x++;
            if (Direction == RelativePosition.West) LastRoomPos.x--;

            return SetRoomAt(LastRoomPos, GetBasicRoom);
        }
    }
    private List<RelativePosition> LookAroundARoom(Vector3Int RoomPosition)
    {
        List<RelativePosition> _moves = new List<RelativePosition>();

        if (RoomPosition.x < this.LayerScale.x - 1 && RoomPosition.x > 0)
        {
            if (this.CurrentLayer[RoomPosition.x + 1, RoomPosition.z].IsActive == false)
            {
                _moves.Add(RelativePosition.East);
            }
            if (this.CurrentLayer[RoomPosition.x - 1, RoomPosition.z].IsActive == false)
            {
                _moves.Add(RelativePosition.West);
            }
        }

        if (RoomPosition.z < this.LayerScale.z - 1 && RoomPosition.z > 0)
        {
            if (this.CurrentLayer[RoomPosition.x, RoomPosition.z + 1].IsActive == false)
            {
                _moves.Add(RelativePosition.North);
            }
            if (this.CurrentLayer[RoomPosition.x, RoomPosition.z - 1].IsActive == false)
            {
                _moves.Add(RelativePosition.South);
            }
        }

        return _moves;
    }
    public RoomData SetRoomAt(Vector3Int Pos, System.Func<Vector3Int, RoomData> F)
    {
        RoomData r = F(Pos);
        this.CurrentLayer[Pos.x, Pos.z] = r;

        return r;
    }

    public RoomData GetStarter(Vector3Int Position)
    {
        RoomData Starter = new RoomData();
        Starter.Configure(Position, RoomData.RoomType.Start);

        return Starter;
    }
    private RoomData GetStairCase(Vector3Int Position)
    {
        RoomData StairCase = new RoomData();
        StairCase.Configure(Position, RoomData.RoomType.Platform);
        return StairCase;
    }
    private RoomData GetBasicRoom(Vector3Int Position)
    {
        RoomData BasicRoom = new RoomData();
        BasicRoom.Configure(Position, RoomData.RoomType.Basic);
        return BasicRoom;
    }
}
