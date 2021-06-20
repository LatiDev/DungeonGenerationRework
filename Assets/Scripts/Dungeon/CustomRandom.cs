using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomRandom
{
    private static byte x = 1, y = 1, z = 1, a = 1;

    public static byte xorshf8()
    {
        byte t = (byte)(x ^ (x << 4));
        x = y;
        y = z;
        z = a;

        a = (byte)(z ^ t ^ (z >> 1) ^ (t << 1));

        return z;
    }
}
