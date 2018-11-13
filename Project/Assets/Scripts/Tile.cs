
using System;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public Vector2Int pPosition;
    public Occupant pOccupant;

    private LineRenderer mLine;
    private bool mMouseOver;

    private void Start()
    {
        mLine = FindObjectOfType<LineRenderer>();
        transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>().text = "X " + pPosition.x;
        transform.GetChild(2).GetChild(0).GetChild(1).GetComponent<Text>().text = "Y " + pPosition.y;
    }

    public void ResetReachable()
    {
        for (int i = 0; i < 6; i++)
        {
            transform.GetChild(1).GetChild(i).gameObject.SetActive(false);
        }
    }

    public void ResetVisibility()
    {
        foreach (var trans in gameObject.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = 9;
        }
    }

    public void IsVisible()
    {
        foreach (var trans in gameObject.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = 0;
        }
    }

    public void IsReachable(Character character)
    {

        for (int j = 0; j < 6; j++)
        {
            Tile tempTile = GridManager.pInstance.GetNeighbour(this, j);
            if (tempTile == null || !character.pReachableTiles.Contains(tempTile))
            {
                transform.GetChild(1).GetChild(j).gameObject.SetActive(true);
            }
        }
    }

    public static int Distance(Tile a, Tile b)
    {
        int dx = Mathf.Abs(a.pPosition.x - b.pPosition.x);
        int dy = Mathf.Abs(a.pPosition.y - b.pPosition.y);
        return dy + Mathf.Max(0, (dx - dy) / 2);
    }

    private void OnMouseEnter()
    {

        if (GameManager.pInstance.pActiveCharacter != null)
        {
            mMouseOver = true;
            mLine.gameObject.SetActive(true);
            mLine.SetPosition(0, transform.position + new Vector3(0, 0.1f, 0));
            mLine.SetPosition(1, GameManager.pInstance.pActiveCharacter.pTile.transform.position + new Vector3(0, 0.1f, 0));

        }

    }

    private void OnMouseExit()
    {
        mMouseOver = false;
        mLine.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (mMouseOver)
        {
            if (Input.GetMouseButtonDown(1))
            {
                int distance = Tile.Distance(GameManager.pInstance.pActiveCharacter.pTile, this);
                for (int k = 1; k <= distance; k++)
                {
                    float Xf = ((GameManager.pInstance.pActiveCharacter.pTile.pPosition.x + ((float)(this.pPosition.x - GameManager.pInstance.pActiveCharacter.pTile.pPosition.x) / distance) * k));
                    float Yf = ((GameManager.pInstance.pActiveCharacter.pTile.pPosition.y + ((float)(this.pPosition.y - GameManager.pInstance.pActiveCharacter.pTile.pPosition.y) / distance) * k));

                    float Xr = Mathf.Round(Xf / 2) * 2;

                    int y = Mathf.RoundToInt(Yf);
                    int x = (int)(Xr);
                    x -= (Mathf.RoundToInt(Xf) % 4 - 2) * (y % 2);
                    Debug.Log("Knoten " + k + " | FloatX = " + Xf + " FloatY = " + Yf + " | RoundedX= " + Xr + " | IntX = " + x + " IntY = " + y);
                }
            }
        }
    }
}
