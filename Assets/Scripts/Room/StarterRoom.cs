using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarterRoom : MonoBehaviour
{
    [SerializeField] private Transform _PlayerSpawnPoint;

    public Transform PlayerSpawnPoint
    {
        get
        {
            return _PlayerSpawnPoint;
        }
    }
}
