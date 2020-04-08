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
    public IEnumerable<RoomData> Generate(Vector3Int Scale, int RoomNumber)
    {
        RoomData[] AllRoom = GenerateMap(Scale, RoomNumber);

        foreach (RoomData rd in SetupWall(AllRoom)) yield return rd;
    }
    public RoomData[] GenerateMap(Vector3Int Scale, int RoomNumber)
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
    public IEnumerable<RoomData> SetupWall(RoomData[] rs)
    {                
        foreach (RoomData rd in rs)
        {
            List<RelativePosition> p = LookAroundARoom(rd.Position, true);

            RoomData NewRoom = rd;
            NewRoom.WallActivated = p;

            yield return NewRoom;            
        }
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
    private List<RelativePosition> LookAroundARoom(Vector3Int RoomPosition, bool Advanced = false)
    {
        List<RelativePosition> _moves = new List<RelativePosition>();

        if (RoomPosition.x < this.LayerScale.x - 1 && RoomPosition.x > 0)
        {
            if (IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.East))
            {
                _moves.Add(RelativePosition.East);
            }
            if (IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.West))
            {
                _moves.Add(RelativePosition.West);
            }
        }

        if (RoomPosition.z < this.LayerScale.z - 1 && RoomPosition.z > 0)
        {
            if (IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.North))
            {
                _moves.Add(RelativePosition.North);
            }
            if (IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.South))
            {
                _moves.Add(RelativePosition.South);
            }
        }

        if (Advanced)
        {
            if (RoomPosition.x < this.LayerScale.x - 1 && RoomPosition.x > 0)
            {
                if (IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.North_East) || 
                    IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.North) ||
                    IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.East))
                {
                    _moves.Add(RelativePosition.North_East);
                }
                if (IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.North_West) ||
                    IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.North) ||
                    IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.West))
                {
                    _moves.Add(RelativePosition.North_West);
                }
            }
            if (RoomPosition.z < this.LayerScale.z - 1 && RoomPosition.z > 0)
            {
                if (IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.South_East) || 
                    IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.East) ||
                    IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.South))
                {
                    _moves.Add(RelativePosition.South_East);
                }
                if (IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.South_West) || 
                    IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.West) ||
                    IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.South))
                {
                    _moves.Add(RelativePosition.South_West);
                }
            }
        }

        return _moves;
    }
    private bool IsNoRoomAt(int RoomX, int RoomZ, RelativePosition p)
    {
        switch (p)
        {
            case RelativePosition.North:
                return this.CurrentLayer[RoomX, RoomZ + 1].IsActive == false;
            case RelativePosition.South:
                return this.CurrentLayer[RoomX, RoomZ - 1].IsActive == false;
            case RelativePosition.East:
                return this.CurrentLayer[RoomX + 1, RoomZ].IsActive == false;
            case RelativePosition.West:
                return this.CurrentLayer[RoomX - 1, RoomZ].IsActive == false;

            case RelativePosition.North_East:
                return this.CurrentLayer[RoomX + 1, RoomZ + 1].IsActive == false;
            case RelativePosition.North_West:
                return this.CurrentLayer[RoomX - 1, RoomZ + 1].IsActive == false;
            case RelativePosition.South_East:
                return this.CurrentLayer[RoomX + 1, RoomZ - 1].IsActive == false;
            case RelativePosition.South_West:
                return this.CurrentLayer[RoomX - 1, RoomZ - 1].IsActive == false;


            default:
                return false;
        }
    }


    private RoomData SetRoomAt(Vector3Int Pos, System.Func<Vector3Int, RoomData> F)
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
