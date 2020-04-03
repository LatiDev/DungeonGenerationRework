using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Mapable))]
public class Room : MonoBehaviour
{
    [SerializeField] private GameObject Base;

    [Header("Walls")]
    [SerializeField] private GameObject FowardWall;
    [SerializeField] private GameObject BackwardWall;
    [SerializeField] private GameObject RigthWall;
    [SerializeField] private GameObject LeftWall;

    [Header("Corner")]
    [SerializeField] private GameObject FowardRigthCorner;
    [SerializeField] private GameObject FowardLeftCorner;
    [SerializeField] private GameObject BackwardRigthCorner;
    [SerializeField] private GameObject BackwardLeftCorner;



}
