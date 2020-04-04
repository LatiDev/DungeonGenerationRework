#define DEBUG

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerateMapable
{
    private GenerationData LastGenerationData;
    

    public void SetBasicData(int sx, int sy, int rn)
    {
        this.LastGenerationData.ScaleX = sx;
        this.LastGenerationData.ScaleY = sy;
        this.LastGenerationData.RoomNumber = rn;
    }
    public GenerationData CreateGenerationData(int lx, int ly, int lz, int rn, Mapable_Info[,] layer)
    {
        GenerationData GD = new GenerationData();
        GD.EndX = lx;
        GD.EndY = ly;
        GD.EndZ = lz;

        GD.RoomNumber = rn;

        GD.Layer = layer;
        GD.LastMapable = layer[lx, ly];

        return GD;
    }
    public Mapable_Info GetStarter(int x, int y)
    {
        Mapable_Info Starter = new Mapable_Info(x, y);
        Starter.IsStarter = true;

        return Starter;
    }
    private Mapable_Info GetStairCase(int x, int y, int l)
    {
        Mapable_Info StairCase = new Mapable_Info(x , y);
        StairCase.IsPlatform = true;

        StairCase.Map_Z = l;

        return StairCase;
    }
    public IEnumerable<GenerationData> Generate(int sx, int sy, int rn, int ln)
    {
        int StarterX = Random.Range(0, sx);
        int StarterY = Random.Range(0, sy);
        Mapable_Info StarterData = this.GetStarter(StarterX, StarterY);

        SetBasicData(sx, sy, rn);

        GenerationData CurrentGenerationData = GenerateNew(StarterX, StarterY, 0, StarterData);

        yield return CurrentGenerationData;

        
        for (int l = 0; l < ln; l++)
        {
            CurrentGenerationData = GenerateFromLastGen(CurrentGenerationData);

            yield return CurrentGenerationData;
        }
        
    }

    private GenerationData GenerateFromLastGen(GenerationData GD)
    {
        GD.EndZ++;
        GenerationData _gd = GenerateNew(GD.EndX, GD.EndY, GD.EndZ, GD.LastMapable);

        return _gd;
    }
    
    private GenerationData GenerateNew(int LastX, int LastY, int LastZ, Mapable_Info Starter)
    {
        int LastRoomX = LastX;
        int LastRoomY = LastY;
        int LastRoomZ = LastZ;

        Mapable_Info[,] Layer = new Mapable_Info[this.LastGenerationData.ScaleX, this.LastGenerationData.ScaleY];

        Layer[LastRoomX, LastRoomY] = Starter;

        for (int _room = 0; _room < this.LastGenerationData.RoomNumber; _room++)
        {
            List<RelativePosition> _moves = new List<RelativePosition>();
            
            if (LastRoomX < this.LastGenerationData.ScaleX -1 && LastRoomX > 0)
            {
                if (Layer[LastRoomX + 1, LastRoomY] == null)
                {
                    _moves.Add(RelativePosition.East);
                }
                if (Layer[LastRoomX - 1, LastRoomY] == null)
                {
                    _moves.Add(RelativePosition.West);
                }
            }

            if (LastRoomY < this.LastGenerationData.ScaleY-1 && LastRoomY > 0)
            {
                if (Layer[LastRoomX, LastRoomY + 1] == null)
                {
                    _moves.Add(RelativePosition.North);
                }
                if (Layer[LastRoomX, LastRoomY - 1] == null)
                {
                    _moves.Add(RelativePosition.South);
                }
            }

            if (_moves.Count == 0)
            {
                Layer[LastRoomX, LastRoomY] = GetStairCase(LastRoomX, LastRoomY, LastRoomZ);
                break;
            }
            else
            {
                RelativePosition Direction = _moves[Random.Range(0, _moves.Count)];

                if (Direction == RelativePosition.North)    LastRoomY++;
                if (Direction == RelativePosition.South)    LastRoomY--;
                if (Direction == RelativePosition.East)     LastRoomX++;
                if (Direction == RelativePosition.West)     LastRoomX--;

                Layer[LastRoomX, LastRoomY] = new Mapable_Info(LastRoomX, LastRoomY);
            }
        }

        this.LastGenerationData = CreateGenerationData(LastRoomX, LastRoomY, LastRoomZ, this.LastGenerationData.RoomNumber, Layer);

        return this.LastGenerationData;
    }
    
}
