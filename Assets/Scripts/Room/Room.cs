using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Mapable))]
public class Room : MonoBehaviour
{
    [SerializeField] private GameObject Base;
    [SerializeField] private GameObject WallParent;

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


    public void SetWall(int poss)
    {
        if ((poss & (int)RelativePosition.North) == (int)RelativePosition.North)
            this.FowardWall.SetActive(true);
        if ((poss & (int)RelativePosition.South) == (int)RelativePosition.South)
            this.BackwardWall.SetActive(true);
        if ((poss & (int)RelativePosition.East) == (int)RelativePosition.East)
            this.RigthWall.SetActive(true);
        if ((poss & (int)RelativePosition.West) == (int)RelativePosition.West)
            this.LeftWall.SetActive(true);
        
        if ((poss & (int)RelativePosition.North_East) == (int)RelativePosition.North_East)
            this.FowardRigthCorner.SetActive(true);
        if ((poss & (int)RelativePosition.North_West) == (int)RelativePosition.North_West)
            this.FowardLeftCorner.SetActive(true);
        if ((poss & (int)RelativePosition.South_East) == (int)RelativePosition.South_East)
            this.BackwardRigthCorner.SetActive(true);
        if ((poss & (int)RelativePosition.South_West) == (int)RelativePosition.South_West)
            this.BackwardLeftCorner.SetActive(true);
    }
}
