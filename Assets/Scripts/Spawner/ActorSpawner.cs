using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorSpawner : MonoBehaviour
{
    [SerializeField] private GameObject ActorPrefab;
    [SerializeField] private Transform ActorParent;

    public void Spawn(Vector3 pos)
    {
        Instantiate(ActorPrefab, pos, Quaternion.identity, ActorParent);
    }
}
