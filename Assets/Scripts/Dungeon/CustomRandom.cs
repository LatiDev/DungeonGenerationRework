using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomRandom
{
    private static System.Random r = new System.Random();

    // min-max as byte
    public static byte Range(byte max)
    {
        return (byte)r.Next(0, max);
    }
}
