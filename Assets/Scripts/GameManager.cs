using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ActorSpawner PlayerSpawner;
    [SerializeField] private LayerGenerator LayerGenerator;
    [SerializeField] private Transform RoomParent;

    [SerializeField] private Transform CameraAnchor;

    private void Update()
    {
        foreach (Transform t in RoomParent)
        {
            Destroy(t.gameObject);
        }
        
        
        LayerGenerator.CreateLayer();
    }
}
