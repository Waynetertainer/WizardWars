using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2 pPosition;

    public bool pIsBlockedUpRight;
    public bool pIsBlockedRight;
    public bool pIsBlockedDownRight;
    public bool pIsBlockedDownLeft;
    public bool pIsBlockedLeft;
    public bool pIsBlockedUpLeft;

    public void AdjustWalls()
    {
        if(pIsBlockedUpRight) transform.GetChild(0).gameObject.SetActive(true);
        if(pIsBlockedRight) transform.GetChild(1).gameObject.SetActive(true);
        if(pIsBlockedDownRight) transform.GetChild(2).gameObject.SetActive(true);
        if(pIsBlockedDownLeft) transform.GetChild(3).gameObject.SetActive(true);
        if(pIsBlockedLeft) transform.GetChild(4).gameObject.SetActive(true);
        if(pIsBlockedUpLeft) transform.GetChild(5).gameObject.SetActive(true);
    }
}
