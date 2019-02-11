using System.Collections.Generic;
using UnityEngine;


public class GridManager : MonoBehaviour
{
    public static GridManager pInstance;

    public GameObject pTilePrefab;
    public Level pCurrentLevel;
    public Material pDefaultMaterial;
    public Material pCollisionMaterial;
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
        DestroyGrid();
        CreateGrid();
        CreateNavigation();

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

                GameObject tempTile = Instantiate(pTilePrefab, new Vector3(j * 1.5f, 0, i * Mathf.Sqrt(3) / 2), pTilePrefab.transform.rotation);
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

    /// <summary>
    /// Checks if a target Tile is visible from a certain point.
    /// </summary>
    /// <param name="startTile">Startingtile from where the vision check is made</param>
    /// <param name="targetTile">Target tile that is looked at.</param>
    /// <param name="visionRange">Range to check.</param>
    /// <returns>Returns the opacity rating of the tiles between start (exclusive) and target (inclusive). If the target is out of range return is Opaque</returns>
    public eVisibility GetVisibilityToTarget(Tile startTile, Tile targetTile, int visionRange)
    {
        if (Tile.Distance(startTile, targetTile) > visionRange) // out of range = opaque
            return eVisibility.Opaque;

        RaycastHit[] mRayHits;
        mRayHits = Physics.SphereCastAll(startTile.transform.position, 0.3f, targetTile.transform.position - startTile.transform.position, Vector3.Distance(startTile.transform.position, targetTile.transform.position));
        //Debug.Log("Collisions to check for Visibility: " + mRayHits.Length.ToString());

        foreach (RaycastHit hit in mRayHits) //search all tile hits for first who makes blocks visibility
        {
            if (hit.transform.gameObject.name.Contains("Tile") && hit.transform.gameObject.GetComponent<Tile>().eVisibility == eVisibility.Opaque) // we only check tile information for occlusion
            {
                return eVisibility.Opaque;
            }
        }

        return eVisibility.Seethrough;
    }

    public eBlockType GetCoverFromTarget(Tile startTile, Tile targetTile)
    {
        RaycastHit mRayHits;
        if (Physics.SphereCast(startTile.transform.position, 0.3f, targetTile.transform.position - startTile.transform.position, out mRayHits, 5f))
            return mRayHits.transform.gameObject.GetComponent<Tile>().pBlockType;
        else
            Debug.Log("No target hit");
        return eBlockType.Empty;
    }

    public List<Tile> GetPathTo(Tile startTile, Tile endTile)
    {
        LinkedList<TilePriority> openToCheck = new LinkedList<TilePriority>();
        Dictionary<Tile, Tile> origins = new Dictionary<Tile, Tile>();
        Dictionary<Tile, int> totalCosts = new Dictionary<Tile, int>();
        openToCheck.AddFirst(new TilePriority(startTile, Tile.Distance(startTile, endTile)));
        origins.Add(startTile, null);
        totalCosts.Add(startTile, 0);


        while (openToCheck.Count > 0) //HACK hard limit on tiles in openlist
        {
            Debug.Assert(openToCheck.Count < 2500, "AI did not find a way from " + startTile.pPosition.ToString() + " to " + endTile.pPosition.ToString() + " in time");
            if (openToCheck.Count > 2500) return null;


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
                if (neighbour == null || neighbour.pBlockType != eBlockType.Empty) continue;
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

    public void HidePath()
    {
        for (int i = 0; i < pPathPainter.transform.childCount - 1; ++i)
        {
            pPathPainter.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void DrawPath(List<Tile> tileList)
    {
        if (tileList == null)
        {
            Debug.Log("Could not find path");
            return;
        }

        if (tileList.Count >= pPathPainter.transform.childCount)
        {
            Debug.LogWarning("Here be dragons...");
            return;
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

    public List<Tile> GetRing(Tile startTile, int radius, bool includeBlocked)
    {
        List<Tile> result = new List<Tile>();
        Tile temp = mGrid[startTile.pPosition.x - 2 * (radius), startTile.pPosition.y];

        for (int i = 0; i < 6; ++i)
        {
            for (int j = 0; j < radius; ++j)
            {
                if (temp != null)
                {
                    if (includeBlocked)                             //if check for blocked tiles not needed, remove all lines with // after it
                    {                                               //
                        result.Add(temp);
                    }                                               //
                    else if (temp.pBlockType == eBlockType.Empty)      //
                    {                                               //
                        result.Add(temp);                           //
                    }                                               //
                }
                temp = GetNeighbour(temp, i);
            }
        }

        return result;
    }

    /// <summary>
    /// Returns all tiles around a center with given radius
    /// </summary>
    public List<Tile> GetCircle(Tile startTile, int radius, bool includeBlocked)
    {
        List<Tile> result = new List<Tile>();
        result.Add(startTile);
        for (int i = 1; i <= radius; i++)
        {
            result.AddRange(GetRing(startTile, i, includeBlocked));
        }
        return result;
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

    public Tile GetCenter()
    {
        int x = mGrid.GetLength(0) / 4;
        int y = mGrid.GetLength(1) / 2;
        if (x % 2 + y % 2 == 1)
            --y; // hack to fix possible null reference
        return mGrid[x, y];
    }

    public Tile GetTileAt(int x, int y)
    {
        return mGrid[x, y];
    }

    public Tile GetTileAt(Vector2Int pos)
    {
        Debug.Assert((pos.x + pos.y) % 2 == 0, "Coordinates broken " + pos.x.ToString() + " " + pos.y.ToString());
        return mGrid[pos.x, pos.y];
    }

    public void CreateNavigation()
    {
        foreach (Transform pTile in transform)
        {

            //Debug.DrawRay(pTile.position, pTile.up, Color.red, 5);
            RaycastHit[] raycastTarget = Physics.SphereCastAll(pTile.position - pTile.up, 0.5f, pTile.up, 3); //TODO: 0.3f radius ist geschätzt. ggfs tweaking nötig.
            Tile tile = pTile.GetComponent<Tile>();

            tile.pBlockType = eBlockType.Empty; // resetting tile settings
            tile.eVisibility = eVisibility.Seethrough;
            if (raycastTarget.Length > 0)
            {
                //Debug.Log("MultiHit " + raycastTarget.Length);
                foreach (RaycastHit hit in raycastTarget)
                {
                    if (hit.transform.name.StartsWith("Tile"))
                    {
                        //Debug.Log("Skipped Tile");
                        continue;
                    }

                    //Debug.Log("Hit -> " + hit.transform.name);
                    if (tile.pBlockType != eBlockType.Blocked) // search if this hit is blocking the tile if it is't already blocked
                    {
                        tile.pBlockType = hit.transform.GetComponent<EditorAssetSettings>().eBlockType;
                    }

                    if (tile.eVisibility != eVisibility.Opaque) // search if this hit is opaque it is't already opaque
                    {
                        tile.eVisibility = hit.transform.GetComponent<EditorAssetSettings>().eVisibility;
                    }
                }


            }
            //TODO: remove after testing? color red to represent changed tiles
            Renderer pRend = pTile.GetComponent<Renderer>();
            if (tile.pBlockType == eBlockType.HalfBlocked || tile.pBlockType == eBlockType.Blocked)
            {

                pRend.material = pCollisionMaterial;
            }
            else
            {
                pRend.material = pDefaultMaterial;
            }
        }
    }

    public void MirrorSpawns()
    {
        // flipping AI Spawn positions
        Vector2Int[] newpositions = new Vector2Int[pCurrentLevel.pAI1Spawner.Length];
        Debug.Log("Flipping AI Spawner positions " + pCurrentLevel.pAI1Spawner.Length.ToString());
        for (int i = 0; i < pCurrentLevel.pAI1Spawner.Length; ++i)
        {
            newpositions[i].x -= pCurrentLevel.pAI1Spawner[i].x - pCurrentLevel.pSize.x * 2;
            newpositions[i].y -= pCurrentLevel.pAI1Spawner[i].y - pCurrentLevel.pSize.y;
        }

        pCurrentLevel.pAI2Spawner = newpositions;
        // flipping Playerpositions
        newpositions = new Vector2Int[pCurrentLevel.pPlayer1Spawns.Length];
        Debug.Log("Flipping Player positions " + pCurrentLevel.pPlayer1Spawns.Length.ToString());
        for (int i = 0; i < pCurrentLevel.pPlayer1Spawns.Length; ++i)
        {
            newpositions[i].x -= pCurrentLevel.pPlayer1Spawns[i].x - pCurrentLevel.pSize.x * 2;
            newpositions[i].y -= pCurrentLevel.pPlayer1Spawns[i].y - pCurrentLevel.pSize.y;
        }

        pCurrentLevel.pPlayer2Spawns = newpositions;
        // flipping already spawned AI positions
        newpositions = new Vector2Int[pCurrentLevel.pAI1Spawns.Length];
        Debug.Log("Flipping AI Spawns " + pCurrentLevel.pAI1Spawns.Length.ToString());
        for (int i = 0; i < pCurrentLevel.pAI1Spawns.Length; ++i)
        {
            newpositions[i].x -= pCurrentLevel.pAI1Spawns[i].x - pCurrentLevel.pSize.x * 2;
            newpositions[i].y -= pCurrentLevel.pAI1Spawns[i].y - pCurrentLevel.pSize.y;
        }

        pCurrentLevel.pAI2Spawns = newpositions;
        //fliping waypoints for PatA
        newpositions = new Vector2Int[pCurrentLevel.pAI1PatrouilleA.Length];
        Debug.Log("Flipping Patrouille A " + pCurrentLevel.pAI1PatrouilleA.Length.ToString());
        for (int i = 0; i < pCurrentLevel.pAI1PatrouilleA.Length; ++i)
        {
            newpositions[i].x -= pCurrentLevel.pAI1PatrouilleA[i].x - pCurrentLevel.pSize.x * 2;
            newpositions[i].y -= pCurrentLevel.pAI1PatrouilleA[i].y - pCurrentLevel.pSize.y;
        }

        pCurrentLevel.pAI2PatrouilleA = newpositions;
        //fliping waypoints for PatB
        newpositions = new Vector2Int[pCurrentLevel.pAI1PatrouilleB.Length];
        Debug.Log("Flipping Patrouille B " + pCurrentLevel.pAI1PatrouilleB.Length.ToString());
        for (int i = 0; i < pCurrentLevel.pAI1PatrouilleB.Length; ++i)
        {
            newpositions[i].x -= pCurrentLevel.pAI1PatrouilleB[i].x - pCurrentLevel.pSize.x * 2;
            newpositions[i].y -= pCurrentLevel.pAI1PatrouilleB[i].y - pCurrentLevel.pSize.y;
        }

        pCurrentLevel.pAI2PatrouilleB = newpositions;
    }

    public void SaveToLevelobject()
    {
        pCurrentLevel.pListGrid = new List<TileStruct>(this.transform.childCount);
        int maxX = 0;
        int maxY = 0;

        foreach (Tile currenTile in transform.GetComponentsInChildren<Tile>())
        {
            maxX = (currenTile.pPosition.x > maxX ? currenTile.pPosition.x : maxX);
            maxY = (currenTile.pPosition.y > maxY ? currenTile.pPosition.y : maxY);
            pCurrentLevel.pListGrid.Add(currenTile.ToTileStruct());
        }

        //pCurrentLevel.pSize = new Vector2Int(maxX/2, maxY);
        //int calculatedTileCount = maxX / 2 * maxY / 2 + ((maxX / 2) + 1) * ((maxY / 2) + 1);
        //Debug.Assert(calculatedTileCount == this.transform.childCount, "Tileanzahl stimmt nicht mit Coordinaten überein. Max Tile Werte " + calculatedTileCount.ToString() + ", Einträge " + this.transform.childCount);
        Debug.Log("Speichern komplett");

    }
}