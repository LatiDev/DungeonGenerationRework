using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mapable_Info
{
    public int Map_X;
    public int Map_Y;
    public int Map_Z;

    public bool IsStarter = false;
    public bool IsPlatform = false;
    public bool IsEnd = false;

    public Mapable_Info(int x, int y)
    {
        this.Map_X = x;
        this.Map_Y = y;
    }

    public override string ToString()
    {
        return $"{Map_X}, {Map_Y} // {IsStarter}, {IsPlatform}, {IsEnd}";
    }
}
