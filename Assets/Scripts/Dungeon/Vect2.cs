using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vect2
{
    private ushort vector;
    public byte x
    {
        get
        {
            return (byte)((vector & 0xFF00) >> 8);
        }
    }
    public byte y
    {
        get
        {
            return (byte)(vector & 0x00FF);
        }
    }
    public Vect2(ushort v)
    {
        this.vector = v;
    }
}
