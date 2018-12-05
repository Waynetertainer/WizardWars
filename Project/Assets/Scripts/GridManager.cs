
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class GridManager : MonoBehaviour
{
    public static GridManager pInstance;

    public GameObject pTilePrefab;
    public GameObject pCharacterPrefab;
    public Vector2Int[,] pSpawnPoints = new Vector2Int[2, 3];
    public Level pCurrentLevel;
    private Tile[,] mGrid;

    private void Awake()
    {
        if (pInstance == null)
        {
            pInstance = this;
        }
        else if (pInstance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        CreateGrid();

        //int i = 0;
        //foreach (Vector2Int position in pSpawnPoints)
        //{
        //    Character character = Instantiate(pCharacterPrefab, new Vector3(
        //        mGrid[i, position.x].gameObject.transform.position.x,
        //        mGrid[i, position.x].gameObject.transform.position.y + 1.2f,
        //        mGrid[i, position.x].gameObject.transform.position.z),
        //        Quaternion.identity).GetComponent<Character>();
        //    character.GetComponent<Character>().pTile = mGrid[i, position.x];
        //    mGrid[i, position.x].pCharacter = character.GetComponent<Character>();
        //    character.pFraction = i < 6 ? eFraction.PC : eFraction.Player;
        //    i += 2;
        //}
    }

    public List<Tile> GetReachableTiles(Tile startTile, int range)
    {
        List<Tile> allTiles = new List<Tile>();
        List<List<Tile>> largeTileList = new List<List<Tile>> { new List<Tile>() };
        largeTileList[0].Add(startTile);
        for (int i = 1; i <= range; i++)
        {
            largeTileList.Add(new List<Tile>());
            foreach (Tile tile in largeTileList[i - 1])
            {
                for (int j = 0; j < 6; j++)
                {
                    Tile tempTile = GetNeighbour(tile, j);
                    if (tempTile != null && tempTile.pBlockType == eBlockType.Empty && !allTiles.Contains(tempTile))
                    {
                        allTiles.Add(tempTile);
                        largeTileList[i].Add(tempTile);
                    }
                }
            }
        }
        return allTiles;
    }

    public void CreateGrid()
    {
        mGrid = new Tile[pCurrentLevel.pSize.x * 2 + 1, pCurrentLevel.pSize.y + 1];
        bool createNew = pCurrentLevel.pListGrid.Count == 0;

        int count = 0;
        for (int j = 0; j < mGrid.GetLength(1); j++)
        {
            for (int i = 0; i < mGrid.GetLength(0); i++)
            {
                if (i % 2 != j % 2) continue;

                GameObject tempTile = Instantiate(pTilePrefab, new Vector3(j * 1.5f, 1, i * Mathf.Sqrt(3) / 2), pTilePrefab.transform.rotation);
                tempTile.transform.parent = this.transform;
                mGrid[i, j] = tempTile.GetComponent<Tile>();

                if (createNew)
                {
                    pCurrentLevel.pListGrid.Add(new TileStruct()
                    {
                        pPosition = new Vector2Int(i, j),
                        pBlockType = eBlockType.Empty,
                        pVisibilty = eVisibility.Seethrough

                        //TODO: Auslesen aus den Assets über dem Tile ähnlich CreateNavigation()
                    });
                }

                mGrid[i, j].pPosition = pCurrentLevel.pListGrid[count].pPosition;
                mGrid[i, j].pBlockType = pCurrentLevel.pListGrid[count].pBlockType;
                mGrid[i, j].eVisibility = pCurrentLevel.pListGrid[count].pVisibilty;
                count++;
            }
        }
    }

    public void DestroyGrid()
    {
        //TODO: Is a NULL-Check needed?
        //if (mGrid == null)
        //    return;


        Debug.Log("Deleting " + transform.childCount + " Tiles");

        for (int i = transform.childCount - 1; i >= 0; --i)
        {
            if (transform.GetChild(i).name.StartsWith("Tile"))
            {
                GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
    }

    public void SaveGrid()
    {
        if (pCurrentLevel == null)
        {
            Debug.LogError("You need an active level to save!");
            return;
        }

        pCurrentLevel.pListGrid.Clear();
        for (int j = 0; j < mGrid.GetLength(1); j++)
        {
            for (int i = 0; i < mGrid.GetLength(0); i++)
            {
                if (i % 2 != j % 2) continue;
                TileStruct ts = new TileStruct()
                {
                    pPosition = mGrid[i, j].pPosition,
                    pBlockType = mGrid[i, j].pBlockType,
                    pVisibilty = mGrid[i, j].eVisibility
                };
                pCurrentLevel.pListGrid.Add(ts);
            }
        }
    }

    //private void CreateGrid(Tile[,] input)
    //{
    //    mGrid = new Tile[input.GetLength(0), input.GetLength(1)];
    //    for (int j = 0; j < mGrid.GetLength(1); j++)
    //    {
    //        for (int i = 0; i < mGrid.GetLength(0); i++)
    //        {
    //            if (i % 2 != j % 2) continue;
    //            GameObject tempTile = Instantiate(pTilePrefab, new Vector3(j * 1.5f, 1, i * Mathf.Sqrt(3) / 2), Quaternion.identity);
    //            mGrid[i, j] = tempTile.GetComponent<Tile>();
    //            mGrid[i, j].pPosition = new Vector2Int(i, j);
    //            //TODO add Occupant
    //        }
    //    }
    //}

    public Tile GetNeighbour(Tile startTile, int direction)
    {
        switch (direction)
        {
            case 0:
                try
                {
                    return mGrid[startTile.pPosition.x + 1, startTile.pPosition.y - 1];
                }
                catch
                {
                    return null;
                }
            case 1:
                try
                {
                    return mGrid[startTile.pPosition.x + 2, startTile.pPosition.y];
                }
                catch
                {
                    return null;
                }
            case 2:
                try
                {
                    return mGrid[startTile.pPosition.x + 1, startTile.pPosition.y + 1];
                }
                catch
                {
                    return null;
                }
            case 3:
                try
                {
                    return mGrid[startTile.pPosition.x - 1, startTile.pPosition.y + 1];
                }
                catch
                {
                    return null;
                }
            case 4:
                try
                {
                    return mGrid[startTile.pPosition.x - 2, startTile.pPosition.y];
                }
                catch
                {
                    return null;
                }
            case 5:
                try
                {
                    return mGrid[startTile.pPosition.x - 1, startTile.pPosition.y - 1];
                }
                catch
                {
                    return null;
                }
            default:
                return null;
        }
    }

    public List<Tile> GetVisibleTiles(Tile startTile, int range)
    {
        List<Tile> allTiles = new List<Tile>();
        List<List<Tile>> largeTileList = new List<List<Tile>> { new List<Tile>() };
        largeTileList[0].Add(startTile);
        for (int i = 1; i <= range; i++)
        {
            largeTileList.Add(new List<Tile>());
            foreach (Tile tile in largeTileList[i - 1])
            {
                for (int j = 0; j < 6; j++)
                {
                    Tile tempTile = GetNeighbour(tile, j);
                    if (tempTile != null && !allTiles.Contains(tempTile))
                    {
                        bool blocked = false;
                        int distance = Tile.Distance(startTile, tempTile);
                        for (int k = 1; k < distance; k++)
                        {
                            float Xf = ((startTile.pPosition.x + ((float)(tempTile.pPosition.x - startTile.pPosition.x) / distance) * k));
                            float Yf = ((startTile.pPosition.y + ((float)(tempTile.pPosition.y - startTile.pPosition.y) / distance) * k));
                            int y = Mathf.RoundToInt(Yf);
                            int x = ((int)(Xf)) + ((y + ((int)Xf) % 2) % 2);
                            if (!(mGrid[x, y] == null || (mGrid[x, y].eVisibility == eVisibility.Seethrough)))
                            {
                                blocked = true;
                            }
                        }
                        if (blocked) continue;
                        allTiles.Add(tempTile);
                        largeTileList[i].Add(tempTile);

                    }
                }
            }
        }
        return allTiles;
    }

    public Tile GetCenter()
    {
        return mGrid[mGrid.GetLength(0) / 4, mGrid.GetLength(1) / 2];
    }

    public Tile GetTileAt(int x, int y)
    {
        return mGrid[x, y];
    }
    public Tile GetTileAt(Vector2Int pos)
    {
        return mGrid[pos.x, pos.y];
    }

    public void CreateNavigation()
    {
        foreach (Transform pTile in transform)
        {

            Debug.DrawRay(pTile.position, pTile.up, Color.red, 5);
            RaycastHit[] raycastTarget = Physics.SphereCastAll(pTile.position, 0.3f, pTile.up, 2); //TODO: 0.3f radius ist geschätzt. ggfs tweaking nötig.
            Tile tile = pTile.GetComponent<Tile>();

            tile.pBlockType = eBlockType.Empty; // resetting tile settings
            tile.eVisibility = eVisibility.Seethrough;
            if (raycastTarget.Length > 0)
            {
                foreach (RaycastHit hit in raycastTarget)
                {
                    if (hit.transform.name.StartsWith("Tile"))
                    {
                        Debug.Log("Skipped Tile");
                        continue;
                    }

                    Debug.Log("Hit -> " + hit.transform.name);
                    if (tile.pBlockType != eBlockType.Blocked) // search if this hit is blocking the tile if it is't already blocked
                    {
                        tile.pBlockType = hit.transform.GetComponent<BuildingBlockSettings>().eBlockType;

                    }

                    if (tile.eVisibility != eVisibility.Opaque) // search if this hit is opaque it is't already opaque
                    {
                        tile.eVisibility = hit.transform.GetComponent<BuildingBlockSettings>().eVisibility;
                    }
                }


            }
            //TODO: remove after testing! color red to represent changed tiles
            if (tile.pBlockType == eBlockType.HalfBlocked || tile.pBlockType == eBlockType.Blocked)
            {
                Renderer pRend = pTile.GetComponent<Renderer>();
                pRend.material.color = Color.red;
            }
        }
    }
}