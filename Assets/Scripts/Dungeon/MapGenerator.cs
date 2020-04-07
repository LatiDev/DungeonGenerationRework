using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator
{
    private LayerData LastGenerationData;
    

    public void SetBasicData(int sx, int sz, int rn)
    {
        this.LastGenerationData.Scale = new Vector3Int(sx, 0, sz);        
        this.LastGenerationData.RoomNumber = rn;
    }
    public LayerData CreateGenerationData(Vector3Int StartPos, Vector3Int EndPos, int rn, RoomData[,] layer)
    {
        LayerData GD = new LayerData();
        GD.RoomNumber = rn;

        GD.StartPositon = StartPos;
        GD.EndPosition = EndPos;

        GD.Layer = layer;
        return GD;
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


    public IEnumerable<LayerData> Generate(Vector3Int Scale, int rn)
    {
        int StarterX = Random.Range(0, Scale.x);
        int StarterZ = Random.Range(0, Scale.z);

        Vector3Int StarterPositon3D = new Vector3Int(StarterX, 0, StarterZ);

        RoomData StarterData = this.GetStarter(StarterPositon3D);
        SetBasicData(Scale.x, Scale.z, rn);
        LayerData CurrentGenerationData = GenerateNew(StarterPositon3D, StarterData);

        yield return CurrentGenerationData;

        /*

        for (int l = 0; l < ln; l++)
        {
            CurrentGenerationData = GenerateFromLastGen(CurrentGenerationData);

            yield return CurrentGenerationData;
        }
        
        */
    }

    private LayerData GenerateFromLastGen(LayerData GD)
    {
        GD.EndPosition.y++;
        LayerData _gd = GenerateNew(GD.EndPosition, GD.LastRoom);

        return _gd;
    }
    private LayerData GenerateNew(Vector3Int StartPosition, RoomData Starter)
    {
        Vector3Int LastRoomPos = StartPosition;

        RoomData[,] Layer = new RoomData[this.LastGenerationData.Scale.x + 10, this.LastGenerationData.Scale.z + 10];

        Layer[LastRoomPos.x, LastRoomPos.z] = Starter;

        for (int _room = 0; _room < this.LastGenerationData.RoomNumber; _room++)
        {
            List<RelativePosition> _moves = new List<RelativePosition>();
            
            if (LastRoomPos.x < this.LastGenerationData.Scale.x - 1 && LastRoomPos.x > 0)
            {
                if (Layer[LastRoomPos.x + 1, LastRoomPos.z].IsActive == false)
                {
                    _moves.Add(RelativePosition.East);
                }
                if (Layer[LastRoomPos.x - 1, LastRoomPos.z].IsActive == false)
                {
                    _moves.Add(RelativePosition.West);
                }
            }

            if (LastRoomPos.z < this.LastGenerationData.Scale.z - 1 && LastRoomPos.z > 0)
            {
                if (Layer[LastRoomPos.x, LastRoomPos.z + 1].IsActive == false)
                {
                    _moves.Add(RelativePosition.North);
                }
                if (Layer[LastRoomPos.x, LastRoomPos.z - 1].IsActive == false)
                {
                    _moves.Add(RelativePosition.South);
                }
            }

            if (_moves.Count == 0)
            {
                //Layer[LastRoomX, LastRoomY] = GetStairCase(LastRoomX, LastRoomY, LastRoomZ);
                break;
            }
            else
            {
                RelativePosition Direction = _moves[Random.Range(0, _moves.Count)];

                if (Direction == RelativePosition.North)    LastRoomPos.z++;
                if (Direction == RelativePosition.South)    LastRoomPos.z--;
                if (Direction == RelativePosition.East)     LastRoomPos.x++;
                if (Direction == RelativePosition.West)     LastRoomPos.x--;


                Layer[LastRoomPos.x, LastRoomPos.z] = GetBasicRoom(LastRoomPos);
            }
        }

        this.LastGenerationData = CreateGenerationData(StartPosition, LastRoomPos, this.LastGenerationData.RoomNumber, Layer);

        return this.LastGenerationData;
    }
    
}
