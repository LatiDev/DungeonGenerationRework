using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vect2
{
    enum VectMask
    {
        x = 0xFF00,
        y = 0x00FF
    }
    
    private ushort _vector;
    public byte x
    {
        get
        {
            return (byte)((this._vector & (ushort)VectMask.x) >> 8);
        }
    }
    public byte y
    {
        get
        {
            return (byte)(this._vector & (ushort)VectMask.y);
        }
    }
    public ushort vector
    {
        get
        {
            return this._vector;
        }
    }
    public Vect2(ushort v)
    {
        this._vector = v;
    }
    public Vect2(byte x, byte y)
    {
        this.EncodeXY(x, y);
    }
    public void EncodeXY(byte x, byte y)
    {
        this._vector = 0;
        this._vector += x;
        this._vector <<= 8;
        this._vector += y;
    }
}
