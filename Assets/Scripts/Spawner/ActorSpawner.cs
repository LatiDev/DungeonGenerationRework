using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorSpawner : MonoBehaviour
{
    [SerializeField] private GameObject ActorPrefab;
    [SerializeField] private Transform ActorParent;

    public GameObject Spawn(Vector3 pos)
    {
        Vector3 realpos = pos;
        //realpos.y += ActorPrefab.transform.localScale.y;

        return Instantiate(ActorPrefab, realpos, Quaternion.identity, ActorParent);
    }
}
