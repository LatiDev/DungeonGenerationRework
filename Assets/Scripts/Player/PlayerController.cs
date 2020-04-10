using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody Rigidbody;
    [SerializeField] [Range(0, 1)] private float Speed;
    
    void Update()
    {
        Vector3 PlayerMoveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            PlayerMoveDirection += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            PlayerMoveDirection += Vector3.back;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            PlayerMoveDirection += Vector3.right;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            PlayerMoveDirection += Vector3.left;
        }

        this.Rigidbody.MovePosition(this.transform.position + PlayerMoveDirection * Speed);
    }
}
