using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ActorSpawner PlayerSpawner;
    [SerializeField] private LayerGenerator LayerGenerator;
    [SerializeField] private Transform RoomParent;

    [SerializeField] private Transform CameraAnchor;
    [SerializeField] private CameraFollow CameraFollower;

    private void Update()
    {
        /*
        int a = 1;
        a <<= 8;
        Debug.Log(a);
        a <<= 8;
        Debug.Log(a);
        a <<= 8;
        Debug.Log(a);
        a <<= 8;
        Debug.Log(a);
        */

        LayerGenerator.GenerateLayer(6, 6);

        /*
        foreach (Transform t in RoomParent) 
            Destroy(t.gameObject);

        LayerGenerationResult LGR = LayerGenerator.GenerateLayer();
        StarterRoom SR = LGR.StarterRoomInstance.GetComponent<StarterRoom>();
        
        */

        /*
        CameraAnchor.position = SR.PlayerSpawnPoint.position;
        GameObject PlayerInstance = PlayerSpawner.Spawn(SR.PlayerSpawnPoint.position);

        CameraFollower.Target = PlayerInstance.transform;
        */
    }
}
