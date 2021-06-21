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
            rd.SetupWalls(this.LookAroundARoom(rd.Position));
            yield return rd;
        }
    }
    public RoomData[,] GenerateMap(Vector2Int Size)
    {
        Vector2Int StartPosition = new Vector2Int(Random.Range(0, Size.x), Random.Range(0, Size.y));

        RoomData[,] Map = new RoomData[Size.x, Size.y];
        Map[StartPosition.x, StartPosition.y] = new RoomData();

        uint PossibleMovement = 0;
        if (StartPosition.x > 0)
        {
            PossibleMovement += (byte)(StartPosition.x);
        }
        if ((Size.x - 1) - StartPosition.x > 0)
        {
            PossibleMovement <<= 8;
            PossibleMovement += (byte)((Size.x - 1) - StartPosition.x);
        }
        if (StartPosition.y > 0)
        {
            PossibleMovement <<= 8;
            PossibleMovement += (byte)(StartPosition.y);
        }
        if ((Size.y - 1) - StartPosition.y > 0)
        {
            PossibleMovement <<= 8;
            PossibleMovement += (byte)((Size.y - 1) - StartPosition.y);
        }

        //PossibleMovement = 0x0000_1100;

        int PerimetreAire = (Size.x * 2) + ((Size.y - 2) * 2); // a revoir
        int DataDirection = 0;
        for (int c = 0; c < PerimetreAire; c++)
        {
            if (PossibleMovement == 0) 
            {
                Debug.Log("Force Exit");
                break;
            }            
            
            /*
            Debug.Log("Next Loop --------------");
            Debug.Log($"StartPosition: {StartPosition}");
            Debug.Log($"PossibleMovement : " +
                $"{(PossibleMovement & 0xFF00_0000) >> 24}," +
                $"{(PossibleMovement & 0x00FF_0000) >> 16}," +
                $"{(PossibleMovement & 0x0000_FF00) >> 8}," +
                $"{(PossibleMovement & 0x0000_00FF)}");
            */

            if ((PossibleMovement & 0x0000_00FF) == 0)
            {
                PossibleMovement >>= 8;
            }
            if ((PossibleMovement & 0x0000_FF00) == 0)
            {
                DataDirection = (int)(PossibleMovement & 0xFFFF_0000);
                PossibleMovement &= 0x0000_FFFF;
                PossibleMovement += (uint)(DataDirection >> 8);
            }
            if ((PossibleMovement & 0x00FF_0000) == 0)
            {
                DataDirection = (int)(PossibleMovement & 0xFF00_0000);
                PossibleMovement &= 0xFF00_FFFF;
                PossibleMovement += (uint)(DataDirection >> 8);
            }

            DataDirection =
                (PossibleMovement & 0x0000_00FF)        > 0 ? 1 : 0 +
                (PossibleMovement & 0x0000_FF00) >> 8   > 0 ? 1 : 0 +
                (PossibleMovement & 0x00FF_0000) >> 16  > 0 ? 1 : 0 +
                (PossibleMovement & 0xFF00_0000) >> 24  > 0 ? 1 : 0;

            DataDirection = Random.Range(0, DataDirection);
            DataDirection = (DataDirection == 0) ? 0 : 8 * DataDirection;
            //Mask = (uint)(1 << DataDirection);
            //DataDirection = (int)((PossibleMovement & byte.MaxValue << DataDirection) >> DataDirection);

            PossibleMovement -= (uint)(1 << DataDirection);
        }

        Debug.Log($"Exit with: " +
            $"{(PossibleMovement & 0xFF00_0000) >> 24}," +
            $"{(PossibleMovement & 0x00FF_0000) >> 16}," +
            $"{(PossibleMovement & 0x0000_FF00) >> 8}," +
            $"{(PossibleMovement & 0x0000_00FF)} == 0 ? {PossibleMovement == 0}");
        
        return Map;
    }


    public RoomData[] GenerateMap(Vector3Int Scale, int RoomNumber)
    {
        this.CurrentLayer = new RoomData[Scale.x, Scale.z];
        this.LayerScale = Scale;

        List<RoomData> AllRoom = new List<RoomData>(RoomNumber); 

        RoomData RoomToPlace = SetRoomAt(GetRandomPosition(this.LayerScale.x, this.LayerScale.z), GetStarter);
        AllRoom.Add(RoomToPlace);
        
        Vector3Int LastPosition = RoomToPlace.Position;
        for (int rn = 0; rn < RoomNumber; rn++)
        {
            RoomToPlace = GetNextRoomAround(LastPosition);

            if (RoomToPlace.Type == RoomData.RoomType.End) break;

            SetRoomAt(RoomToPlace);
            AllRoom.Add(RoomToPlace);

            LastPosition = RoomToPlace.Position;
        }

        AllRoom.Remove(LastRoom);

        RoomToPlace = GetEndRoom(LastPosition);
        AllRoom.Add(RoomToPlace);
        SetRoomAt(RoomToPlace);

        return AllRoom.ToArray();
    }
    private RoomData GetNextRoomAround(Vector3Int LastRoomPos)
    {
        Vector3Int Pos = LastRoomPos;
                
        int _moves = LookAroundARoom(Pos);

        if (_moves == 0)
        {
            return GetEndRoom(Pos);
        }
        else
        {
            long RandomNumber = CustomRandom.xorshf8();
            int FormatedNumber = 
                ((RandomNumber % 2 == 0) ? (int)RelativePosition.North : 0) + 
                ((RandomNumber % 3 == 0) ? (int)RelativePosition.South : 0) +
                ((RandomNumber % 5 == 0) ? (int)RelativePosition.East  : 0) +
                ((RandomNumber % 7 == 0) ? (int)RelativePosition.West  : 0);

            RelativePosition Direction = (RelativePosition)(_moves & FormatedNumber);

            if (Direction == RelativePosition.North) Pos.z++;
            if (Direction == RelativePosition.South) Pos.z--;
            if (Direction == RelativePosition.East)  Pos.x++;
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
        return RoomPosition == LastRoom.Position;
    }
    private int LookAroundARoom(Vector3Int RoomPosition)
    {
        return LookAroundSimpleOnly(RoomPosition) + LookAroundAdvancedOnly(RoomPosition);
    }
    private int LookAroundSimpleOnly(Vector3Int RoomPosition)
    {
        int _moves = 0;

        if (RoomPosition.x < this.LayerScale.x - 1 && RoomPosition.x > 0)
        {
            if (IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.East))
            {
                _moves += (int)RelativePosition.East;
            }
            if (IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.West))
            {
                _moves += (int)RelativePosition.West;
            }
        }
        
        if (RoomPosition.z < this.LayerScale.z - 1 && RoomPosition.z > 0)
        {
            if (IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.North))
            {
                _moves += (int)RelativePosition.North;
            }
            if (IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.South))
            {
                _moves += (int)RelativePosition.South;
            }
        }

        return _moves;
    }
    private int LookAroundAdvancedOnly(Vector3Int RoomPosition)
    {
        int _moves = 0;

        if ((RoomPosition.x < this.LayerScale.x - 1 && RoomPosition.x > 0) &&
            (RoomPosition.z < this.LayerScale.z - 1 && RoomPosition.z > 0))
        {
            if (IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.North_East) | 
                IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.North) |
                IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.East))
            {
                _moves += (int)RelativePosition.North_East;
            }
            if (IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.North_West) | 
                IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.North) |
                IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.West))
            {
                _moves += (int)RelativePosition.North_West;
            }
            if (IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.South_East) | 
                IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.South) |
                IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.East))
            {
                _moves += (int)RelativePosition.South_East;
            }
            if (IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.South_West) |
                IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.West) | 
                IsNoRoomAt(RoomPosition.x, RoomPosition.z, RelativePosition.South))
            {
                _moves += (int)RelativePosition.South_West;
            }
        }

        return _moves;
    }
    private bool IsNoRoomAt(int RoomX, int RoomZ, RelativePosition p)
    {
        switch (p)
        {
            case RelativePosition.North:
                return !this.CurrentLayer[RoomX, RoomZ + 1].IsActive;
            case RelativePosition.South:
                return !this.CurrentLayer[RoomX, RoomZ - 1].IsActive;
            case RelativePosition.East:
                return !this.CurrentLayer[RoomX + 1, RoomZ].IsActive;
            case RelativePosition.West:
                return !this.CurrentLayer[RoomX - 1, RoomZ].IsActive;

            case RelativePosition.North_East:
                return !this.CurrentLayer[RoomX + 1, RoomZ + 1].IsActive;
            case RelativePosition.North_West:
                return !this.CurrentLayer[RoomX - 1, RoomZ + 1].IsActive;
            case RelativePosition.South_East:
                return !this.CurrentLayer[RoomX + 1, RoomZ - 1].IsActive;
            case RelativePosition.South_West:
                return !this.CurrentLayer[RoomX - 1, RoomZ - 1].IsActive;

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
