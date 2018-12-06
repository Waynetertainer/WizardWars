
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
    public GameObject pPathPainter;

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


        foreach (Vector2Int position in pSpawnPoints)
        {
            GameObject character = Instantiate(pCharacterPrefab, new Vector3(
                mGrid[position.y, position.x].gameObject.transform.position.x,
                mGrid[position.y, position.x].gameObject.transform.position.y + 1.2f,
                mGrid[position.y, position.x].gameObject.transform.position.z),
                Quaternion.identity);
            character.GetComponent<Character>().pTile = mGrid[position.y, position.x];
            mGrid[position.y, position.x].pOccupant = character.GetComponent<Character>();
        }
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
                    if (tempTile != null && tempTile.pOccupant == null && !allTiles.Contains(tempTile))
                    {
                        allTiles.Add(tempTile);
                        largeTileList[i].Add(tempTile);
                    }
                }
            }
        }
        return allTiles;
    }

    private void CreateGrid()
    {
        mGrid = new Tile[101, 51];
        for (int j = 0; j < mGrid.GetLength(1); j++)
        {
            for (int i = 0; i < mGrid.GetLength(0); i++)
            {
                if (i % 2 != j % 2) continue;
                GameObject tempTile = Instantiate(pTilePrefab, new Vector3(j * 1.5f, 1, i * Mathf.Sqrt(3) / 2), pTilePrefab.transform.rotation);
                tempTile.transform.parent = this.transform;
                mGrid[i, j] = tempTile.GetComponent<Tile>();
                mGrid[i, j].pPosition = new Vector2Int(i, j);
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
                        bool Blocked = false;
                        int distance = Tile.Distance(startTile, tempTile);
                        for (int k = 1; k < distance; k++)
                        {
                            float Xf = ((startTile.pPosition.x + ((float)(tempTile.pPosition.x - startTile.pPosition.x) / distance) * k));
                            float Yf = ((startTile.pPosition.y + ((float)(tempTile.pPosition.y - startTile.pPosition.y) / distance) * k));
                            int y = Mathf.RoundToInt(Yf);
                            int x = ((int)(Xf)) + ((y + ((int)Xf) % 2) % 2);
                            if (!(mGrid[x, y] == null || mGrid[x, y].pOccupant == null))
                            {
                                Blocked = true;
                            }
                        }
                        if (Blocked) continue;
                        allTiles.Add(tempTile);
                        largeTileList[i].Add(tempTile);

                    }
                }
            }
        }
        return allTiles;
    }



    public List<Tile> GetPathTo(Tile startTile, Tile endTile)
    {
        LinkedList<TilePriority> openToCheck = new LinkedList<TilePriority>();
        Dictionary<Tile, Tile> origins = new Dictionary<Tile, Tile>();
        Dictionary<Tile, int> totalCosts = new Dictionary<Tile, int>();
        openToCheck.AddFirst(new TilePriority(startTile, Tile.Distance(startTile, endTile)));
        origins.Add(startTile, null);
        totalCosts.Add(startTile, 0);


        while (openToCheck.Count > 0)
        {
            Tile activeTile = openToCheck.First.Value.tile;
            openToCheck.RemoveFirst();

            if (activeTile == endTile)
            {
                List<Tile> result = new List<Tile>();
                while (activeTile != startTile)
                {
                    result.Add(activeTile);
                    activeTile = origins[activeTile];
                }
                return result;
            }

            foreach (Tile neighbour in activeTile.GetNeighbours())
            {
                if (neighbour == null || neighbour.pOccupant != null) continue;
                int newCost = totalCosts[activeTile] + 1;
                if (!totalCosts.ContainsKey(neighbour) || totalCosts[neighbour] >= newCost)
                {
                    totalCosts[neighbour] = newCost;
                    TilePriority temp = new TilePriority(neighbour, newCost + Tile.Distance(neighbour, endTile));
                    if (openToCheck.Count > 0)
                    {
                        for (LinkedListNode<TilePriority> recentNode = openToCheck.First; recentNode != null; recentNode = recentNode.Next)
                        {
                            if (recentNode.Value.priority > temp.priority)
                            {
                                openToCheck.AddBefore(recentNode, temp);
                                break;
                            }
                        }
                    }
                    else
                    {
                        openToCheck.AddFirst(temp);
                    }
                    origins[neighbour] = activeTile;
                }
            }
        }
        return null;
    }

    public void DrawPath(List<Tile> tileList)
    {
        if (tileList.Count >= 20)
        {
            throw new ArgumentOutOfRangeException();
        }
        for (int i = 1; i < tileList.Count; i++)
        {
            GameObject lineObject = pPathPainter.transform.GetChild(i).gameObject;
            LineRenderer lineRenderer = lineObject.GetComponent<LineRenderer>();

            lineObject.SetActive(true);
            lineRenderer.SetPosition(0, tileList[i - 1].transform.position);
            lineRenderer.SetPosition(1, tileList[i].transform.position);
        }
    }

    private struct TilePriority
    {
        public Tile tile;
        public int priority;

        public TilePriority(Tile inTile, int inPriority)
        {
            tile = inTile;
            priority = inPriority;
        }
    }
}
