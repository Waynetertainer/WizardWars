using Boo.Lang;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject pTilePrefab;
    public GameObject pCharacterPrefab;

    public Vector2Int[] pSpawnPoints = new Vector2Int[3];

    private Tile[,] mGrid;

    private void CreateNewGrid()
    {
        mGrid = new Tile[30, 15];
        for (int j = 0; j < mGrid.GetLength(1); j++)
        {
            for (int i = 0; i < mGrid.GetLength(0); i++)
            {
                if (i % 2 != j % 2) continue;
                GameObject tempTile = Instantiate(pTilePrefab, new Vector3(j * 1.5f, (float)GameManager.pInstance.pRandom.NextDouble() / 2, i * Mathf.Sqrt(3) / 2), Quaternion.identity);
                //tempTile.transform.localScale += new Vector3(0, (float)GameManager.pInstance.pRandom.NextDouble() / 2, 0);
                mGrid[i, j] = tempTile.GetComponent<Tile>();
                mGrid[i, j].pPosition = new Vector2(i, j);

                if (j == 0)
                {
                    mGrid[i, j].pIsBlockedUpRight = true;
                    mGrid[i, j].pIsBlockedUpLeft = true;
                }

                if (j == mGrid.GetLength(1) - 1)
                {
                    mGrid[i, j].pIsBlockedDownRight = true;
                    mGrid[i, j].pIsBlockedDownLeft = true;
                }

                if (i == 0)
                {
                    mGrid[i, j].pIsBlockedDownLeft = true;
                    mGrid[i, j].pIsBlockedLeft = true;
                    mGrid[i, j].pIsBlockedUpLeft = true;
                }

                if (i == 1)
                {
                    mGrid[i, j].pIsBlockedLeft = true;
                }

                if (i == mGrid.GetLength(0) - 1)
                {
                    mGrid[i, j].pIsBlockedDownRight = true;
                    mGrid[i, j].pIsBlockedRight = true;
                    mGrid[i, j].pIsBlockedUpRight = true;
                }

                if (i == mGrid.GetLength(0) - 2)
                {
                    mGrid[i, j].pIsBlockedRight = true;
                }

                mGrid[i,j].AdjustWalls();
            }
        }
    }

    private void Start()
    {
        CreateNewGrid();
        foreach (Vector2Int position in pSpawnPoints)
        {
            Instantiate(pCharacterPrefab, new Vector3(
                mGrid[position.y, position.x].gameObject.transform.position.x,
                mGrid[position.y, position.x].gameObject.transform.position.y+2,
                mGrid[position.y, position.x].gameObject.transform.position.z),
                Quaternion.identity);
        }
    }

    public List<Tile> GetReachableTiles(Tile startTile,int range)
    {
        List<Tile> allTiles = new List<Tile>();
        List<List<Tile>> largeTileList = new List<List<Tile>> {new List<Tile>().Add(startTile)};
        for (int i = 1; i < range; i++)
        {
            largeTileList.Add(new List<Tile>());
            foreach (Tile tile in largeTileList[i-1])
            {
                for (int j = 0; j < 6; j++)
                {
                    Tile tempTile = GetNeighbour(tile, j);
                }
            }
        }
        return allTiles;
    }

    private Tile GetNeighbour(Tile startTile,int direction)
    {
        switch (direction)
        {
            case 0:
                if (startTile.pPosition.y != 0)
                {
                    if (mGrid.GetLength(0) % 2 == 1)
                    {

                    }
                }
                break;
        }
    }
}
