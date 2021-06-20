using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RelativePosition
{
    None = 0,
    
    North = 1,
    East = 2,
    South = 4,
    West = 8,
    
    North_West = 16,
    North_East = 32,
    South_West = 64,
    South_East = 128,
}