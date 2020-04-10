using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorSpawnData : MonoBehaviour
{
    [SerializeField] private float _ActorSpawnDelta = 0; 
    public float ActorSpawnDelta
    {
        get
        {
            if (_ActorSpawnDelta == 0)
                return this.transform.lossyScale.y / 2;
            else
                return _ActorSpawnDelta;
        }
    }
}
