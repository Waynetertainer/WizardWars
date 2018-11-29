using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[ExecuteInEditMode]
public class Tile : MonoBehaviour
{
    public Vector2Int pPosition;
    public int pCharacterId = -1;
    public eBlockType pBlockType;
    public eVisibility pVisibilty;

    protected Color mColor;
    protected bool mMouseOver;
    private LineRenderer mLine;

    private void Start()
    {
        mLine = FindObjectOfType<LineRenderer>();
        transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = "X " + pPosition.x;
        transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Text>().text = "Y " + pPosition.y;
    }

    public void ResetReachable()
    {
        for (int i = 0; i < 6; i++)
        {
            transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
        }
    }

    public void ResetVisibility()
    {
        foreach (var trans in gameObject.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = 9;
        }
        //if (pOccupant != null)
        //{
        //    pOccupant.GetComponent<MeshRenderer>().enabled = false;
        //}
    }

    public void IsVisible()
    {
        foreach (var trans in gameObject.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = 0;
        }

        if (pCharacterId != -1)
        {
            EntityManager.pInstance.GetCharacterForId(pCharacterId).GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public void IsReachable(Character character)
    {
        for (int j = 0; j < 6; j++)
        {
            Tile tempTile = GridManager.pInstance.GetNeighbour(this, j);
            if (tempTile == null || !character.pReachableTiles.Contains(tempTile))
            {
                transform.GetChild(0).GetChild(j).gameObject.SetActive(true);
            }
        }
    }

    public static int Distance(Tile a, Tile b)
    {
        int dx = Mathf.Abs(a.pPosition.x - b.pPosition.x);
        int dy = Mathf.Abs(a.pPosition.y - b.pPosition.y);
        return dy + Mathf.Max(0, (dx - dy) / 2);
    }

    protected virtual void OnMouseEnter()
    {
        switch (GameManager.pInstance.pGameState)
        {
            case eGameState.Move:
                mLine.gameObject.SetActive(true);
                mLine.SetPosition(0, transform.position + new Vector3(0, 0.1f, 0));
                mLine.SetPosition(1, GameManager.pInstance.pActiveCharacter.pTile.transform.position + new Vector3(0, 0.1f, 0)); break;
            case eGameState.FireSkill:
                mColor = GetComponent<Renderer>().material.color;
                GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                break;
            case eGameState.FireUnique:
                mColor = GetComponent<Renderer>().material.color;
                GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                for (int i = 0; i < 6; ++i)
                {
                    Tile t = GridManager.pInstance.GetNeighbour(this, i);
                    if (t != null)
                        t.GetComponent<Renderer>().material.SetColor("_Color", Color.red); ;
                }
                break;
        }
        if (GameManager.pInstance.pGameState == eGameState.Move)
        {
            mMouseOver = true;
            mLine.gameObject.SetActive(true);
            mLine.SetPosition(0, transform.position + new Vector3(0, 0.1f, 0));
            mLine.SetPosition(1, GameManager.pInstance.pActiveCharacter.pTile.transform.position + new Vector3(0, 0.1f, 0));
        }
    }

    protected virtual void OnMouseExit()
    {
        mLine.gameObject.SetActive(false);
        switch (GameManager.pInstance.pGameState)
        {
            case eGameState.Move:
                mMouseOver = false;
                break;
            case eGameState.FireSkill:
                GetComponent<Renderer>().material.SetColor("_Color", mColor);
                break;
            case eGameState.FireUnique:
                GetComponent<Renderer>().material.SetColor("_Color", mColor);
                for (int i = 0; i < 6; ++i)
                {
                    Tile t = GridManager.pInstance.GetNeighbour(this, i);
                    if (t != null)
                        t.GetComponent<Renderer>().material.SetColor("_Color", mColor); ;
                }
                break;
        }

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
                    int y = Mathf.RoundToInt(Yf);
                    int x = ((int)(Xf)) + ((y + ((int)Xf) % 2) % 2);
                    Debug.Log("Knoten " + k + " | FloatX = " + Xf + " FloatY = " + Yf + " | RoundedX= " + "NI" + " | IntX = " + x + " IntY = " + y);
                }
            }
        }
    }
}

[Serializable]
public struct TileStruct
{
    public Vector2Int pPosition;
    public eBlockType pBlockType;
    public eVisibility pVisibilty;
}