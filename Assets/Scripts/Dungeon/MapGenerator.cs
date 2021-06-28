using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator
{
    private RoomData[,] CurrentLayer;
    private Vector3Int LayerScale;

    private RoomData LastRoom;

    // 0xxxyy
    enum SizeMask : ushort
    {
        x = 0xFF00,
        y = 0x00FF,
    }
    // 0xxxXX_yyYY
    enum MovementMask : uint
    {
        x =         0xFF00_0000,
        delta_x =   0x00FF_0000,
        y =         0x0000_FF00,
        delta_y =   0x0000_00FF,
    }
    enum MovementCodeMask : byte
    {
        x =         0xAA,
        delta_x =   0xBB,
        y =         0xCC,
        delta_y =   0xDD,
    }
    enum RoomCode : byte
    {
        start =         0xAA,
        simpleRoom =    0xBB,
        end =           0xCC
    }

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
    public void GenerateMap(byte Size)
    {
        int RandVariable;
        uint Mask;
        ushort PerimetreAire;
        byte PositionX;
        byte PositionY;
        ushort cells;
        uint PossibleMovement;

        byte[,] Map = new byte[Size, Size];

        //PositionX = (byte)Random.Range(0, Size);
        //PositionY = (byte)Random.Range(0, Size);

        PositionX = 3;
        PositionY = 2;


        Map[PositionX, PositionY] = (byte)RoomCode.start;

        PossibleMovement = 0;
        if (PositionX > 0)
            PossibleMovement += (byte)(PositionX);
        PossibleMovement <<= 8;
        if ((Size - 1) - PositionX > 0)
            PossibleMovement += (byte)((Size - 1) - PositionX);
        PossibleMovement <<= 8;
        if (PositionY > 0)
            PossibleMovement += (byte)(PositionY);
        PossibleMovement <<= 8;
        if ((Size - 1) - PositionY > 0)
            PossibleMovement += (byte)((Size - 1) - PositionY);

        PerimetreAire = (ushort)(Size > 1? (Size-1) * 4 : 1);
        for (cells = 0; cells < PerimetreAire; cells++)
        {
            //Debug.Log("Loop ------------------------------------");
            if (PossibleMovement == 0) break;

            /*
            Debug.Log($"PossibleMovement: " +
            $"x:{(PossibleMovement & (uint)MovementMask.x) >> 24}," +
            $"dx:{(PossibleMovement & (uint)MovementMask.delta_x) >> 16}," +
            $"y:{(PossibleMovement & (uint)MovementMask.y) >> 8}," +
            $"dy:{(PossibleMovement & (uint)MovementMask.delta_y)}");
            */

            RandVariable =
                ((PossibleMovement & (uint)MovementMask.delta_y)    == 0 ? 0 : 1) +
                ((PossibleMovement & (uint)MovementMask.y)          == 0 ? 0 : 1) +
                ((PossibleMovement & (uint)MovementMask.delta_x)    == 0 ? 0 : 1) +
                ((PossibleMovement & (uint)MovementMask.x)          == 0 ? 0 : 1);

            //Debug.Log($"Is Valid? : {RandVariable}");
            if (RandVariable == 4)
            {
                RandVariable = (byte)Random.Range(0, RandVariable);
                RandVariable = (RandVariable == 0) ? 0 : 8 * RandVariable;
                PossibleMovement -= (uint)(1 << RandVariable);
            }
            else
            {
                Mask = 0;
                for (uint m = 0xFF; m != 0x0000_0000; m <<= 8)
                {
                    if ((PossibleMovement & m) == 0)
                    {
                        if (m == 0x000_00FF)
                            Mask = 0xFFFF_FF00;
                        else if (m == 0x0000_FF00)
                            Mask = 0xFFFF_0000;
                        else if (m == 0x00FF_0000)
                            Mask = 0xFF00_0000;
                        RandVariable = (int)(PossibleMovement & Mask);
                        PossibleMovement &= (0xFFFF_FFFF ^ Mask);
                        PossibleMovement += (uint)(RandVariable) >> 8;
                    }                                        
                }

                /*
                Debug.Log($"Rectify PossibleMovement: " +
                $"x:{(PossibleMovement & (uint)MovementMask.x) >> 24}," +
                $"dx:{(PossibleMovement & (uint)MovementMask.delta_x) >> 16}," +
                $"y:{(PossibleMovement & (uint)MovementMask.y) >> 8}," +
                $"dy:{(PossibleMovement & (uint)MovementMask.delta_y)}");
                */                

                RandVariable =
                    ((PossibleMovement & (uint)MovementMask.delta_y)          > 0 ? 1 : 0) +
                    ((PossibleMovement & (uint)MovementMask.y)          >> 8  > 0 ? 1 : 0) +
                    ((PossibleMovement & (uint)MovementMask.delta_x)    >> 16 > 0 ? 1 : 0) +
                    ((PossibleMovement & (uint)MovementMask.x)          >> 24 > 0 ? 1 : 0);


                RandVariable = (byte)Random.Range(0, RandVariable);
                RandVariable = (RandVariable == 0) ? 0 : 8 * RandVariable;
                PossibleMovement -= (uint)(1 << RandVariable);

                /*

                Debug.Log($"-1 PossibleMovement: " +
                $"x:{(PossibleMovement & (uint)MovementMask.x) >> 24}," +
                $"dx:{(PossibleMovement & (uint)MovementMask.delta_x) >> 16}," +
                $"y:{(PossibleMovement & (uint)MovementMask.y) >> 8}," +
                $"dy:{(PossibleMovement & (uint)MovementMask.delta_y)}");
             
                Mask >>= 8;
                RandVariable = (int)(PossibleMovement & Mask);
                PossibleMovement &= (0xFFFF_FFFF - Mask);
                PossibleMovement += (uint)(RandVariable << 8);


                Debug.Log($"Recover PossibleMovement: " +
                $"x:{(PossibleMovement & (uint)MovementMask.x) >> 24}," +
                $"dx:{(PossibleMovement & (uint)MovementMask.delta_x) >> 16}," +
                $"y:{(PossibleMovement & (uint)MovementMask.y) >> 8}," +
                $"dy:{(PossibleMovement & (uint)MovementMask.delta_y)}");

                */
            }

            /*
            Debug.Log($"END PossibleMovement: " +
            $"x:{(PossibleMovement & (uint)MovementMask.x) >> 24}," +
            $"dx:{(PossibleMovement & (uint)MovementMask.delta_x) >> 16}," +
            $"y:{(PossibleMovement & (uint)MovementMask.y) >> 8}," +
            $"dy:{(PossibleMovement & (uint)MovementMask.delta_y)}");
            */

            Map[PositionX, PositionY] = (byte)RoomCode.simpleRoom;
        }

        //return Map;
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
            long RandomNumber = 0;
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
