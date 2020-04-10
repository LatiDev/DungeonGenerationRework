using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLookCursor : MonoBehaviour
{
    private void Update()
    {
        Ray cursorayfromcamera = Camera.main.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(cursorayfromcamera.origin, cursorayfromcamera.direction);

        Ray r = cursorayfromcamera;
        RaycastHit Hit;
        if (Physics.Raycast(r, out Hit))
        {
            Vector3 hitedpoint = Hit.point;
            hitedpoint.y = this.transform.position.y;

            Vector3 lookpoint = hitedpoint;

            this.transform.LookAt(lookpoint);
        }
        else
        {
            Vector3 p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            p.y = this.transform.position.y;

            this.transform.LookAt(p);
        }
    }
}
