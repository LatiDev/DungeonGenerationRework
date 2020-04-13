using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator
{
    private RoomData[,] CurrentLayer;
    private Vector3Int LayerScale;

    private RoomData LastRoom;


    private Vector3Int GetRandomPosition(int maxx, int maxz) 
    {
        int Randx = Random.Range(0, maxx);
        int RandZ = Random.Range(0, maxz);

        return new Vector3Int(Randx, 0, RandZ);
    }
    public IEnumerable<RoomData> Generate(Vector3Int Scale, int RoomNumber)
    {
        RoomData[] AllRoom = GenerateMap(Scale, RoomNumber);

        foreach (RoomData rd in AllRoom)
        {
            List<RelativePosition> AroundSimple = this.LookAroundARoom(rd.Position);
            List<RelativePosition> AroundAdvanced = this.LookAroundARoom(rd.Position, true);

            rd.SetupWalls(AroundAdvanced);


            yield return rd;
        }
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
            RoomData RoomToPlace = GetNextRoomAround(LastPosition);

            if (RoomToPlace.Type == RoomData.RoomType.End)
            {
                ReplaceLastRoomBy(RoomToPlace);
                AllRoom.Remove(LastRoom);
                AllRoom.Add(RoomToPlace);
                break;
            }

            SetRoomAt(RoomToPlace);
            AllRoom.Add(RoomToPlace);

            LastPosition = RoomToPlace.Position;
        }

        return AllRoom.ToArray();
    }
    private RoomData GetNextRoomAround(Vector3Int LastRoomPos)
    {
        Vector3Int Pos = LastRoomPos;
                
        List<RelativePosition> _moves = LookAroundARoom(Pos);

        if (_moves.Count == 0)
        {
            //return SetRoomAt(LastRoomPos, GetStairCase); <- TODO: Change this
            return GetEndRoom(Pos);
        }
        else
        {
            RelativePosition Direction = _moves[Random.Range(0, _moves.Count)];

            if (Direction == RelativePosition.North) Pos.z++;
            if (Direction == RelativePosition.South) Pos.z--;
            if (Direction == RelativePosition.East) Pos.x++;
            if (Direction == RelativePosition.West) Pos.x--;

            return GetBasicRoom(Pos);
        }
    }
    public void ReplaceLastRoomBy(RoomData rd)
    {
        this.CurrentLayer[LastRoom.Position.x, LastRoom.Position.z] = rd;
    }
    public bool IsRoomOverlap(Vector3Int RoomPosition)
    {
        //Debug.Log($"{RoomPosition} == {LastRoomPosition}");
        return RoomPosition == LastRoom.Position;
    }
    private List<RelativePosition> LookAroundARoom(Vector3Int RoomPosition, bool Advanced = false)
    {
        List<RelativePosition> _moves = new List<RelativePosition>();

        _moves.AddRange(LookAroundSimpleOnly(RoomPosition));
        if (Advanced) _moves.AddRange(LookAroundAdvancedOnly(RoomPosition));

        return _moves;
    }
    private List<RelativePosition> LookAroundSimpleOnly(Vector3Int RoomPosition)
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

        return _moves;
    }
    private List<RelativePosition> LookAroundAdvancedOnly(Vector3Int RoomPosition)
    {
        List<RelativePosition> _moves = new List<RelativePosition>();

        if (RoomPosition.x < this.LayerScale.x - 1 && RoomPosition.x > 0)
        {
            if (IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.North_East, RelativePosition.North, RelativePosition.East))
            {
                _moves.Add(RelativePosition.North_East);
            }
            if (IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.North_West, RelativePosition.North, RelativePosition.West))
            {
                _moves.Add(RelativePosition.North_West);
            }
        }
        if (RoomPosition.z < this.LayerScale.z - 1 && RoomPosition.z > 0)
        {
            if (IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.South_East, RelativePosition.South, RelativePosition.East))
            {
                _moves.Add(RelativePosition.South_East);
            }
            if (IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.South_West, RelativePosition.West, RelativePosition.South))
            {
                _moves.Add(RelativePosition.South_West);    
            }
        }

        return _moves;
    }
    private bool IsNoRoomAt(int RoomX, int RoomZ, params RelativePosition[] p)
    {
        bool DefaultStat = false;        
        foreach (RelativePosition rl in p) DefaultStat |= IsNoRoomAt(RoomX, RoomZ, rl);

        return DefaultStat;
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

        //Debug.Log($"LastPosition set to: {r.Position}");

        this.LastRoom = r;
        return r;
    }
    private void SetRoomAt(RoomData rd)
    {
        this.CurrentLayer[rd.Position.x, rd.Position.z] = rd;
        this.LastRoom = rd;
    }
    public RoomData GetEndRoom(Vector3Int Position)
    {
        RoomData End = new RoomData();
        End.Configure(Position, RoomData.RoomType.End);

        return End;
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
