using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [HideInInspector] public Transform Target;

    private void Update()
    {
        this.transform.position = Target.position;
    }
}
