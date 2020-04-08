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

    private void Start()
    {
        //if (WallParent.activeSelf) { WallParent.SetActive(false); }
    }
    public void SetWall(List<RelativePosition> poss)
    {
        foreach (RelativePosition pos in poss) { WallSetup(pos); }
    }
    private void WallSetup(RelativePosition rp)
    {
        switch (rp)
        {
            case RelativePosition.North:
                this.FowardWall.SetActive(true);
                break;
            case RelativePosition.South:
                this.BackwardWall.SetActive(true);
                break;
            case RelativePosition.East:
                this.RigthWall.SetActive(true);
                break;
            case RelativePosition.West:
                this.LeftWall.SetActive(true);
                break;

            case RelativePosition.North_East:
                this.FowardRigthCorner.SetActive(true);
                break;
            case RelativePosition.North_West:
                this.FowardLeftCorner.SetActive(true);
                break;
            case RelativePosition.South_East:
                this.BackwardRigthCorner.SetActive(true);
                break;
            case RelativePosition.South_West:
                this.BackwardLeftCorner.SetActive(true);
                break;
        }
    }
}
