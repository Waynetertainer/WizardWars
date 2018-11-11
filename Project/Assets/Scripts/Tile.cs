
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int pPosition;
    public Occupant pOccupant;

    public void ResetTile()
    {
        for (int i = 0; i < 6; i++)
        {
            transform.GetChild(1).GetChild(i).gameObject.SetActive(false);
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

    public static int Distance (Tile a,Tile b)
    {
        int dx = Mathf.Abs(a.pPosition.x - b.pPosition.x);
        int dy = Mathf.Abs(a.pPosition.y - b.pPosition.y);
        return dy + Mathf.Max(0, (dx - dy) / 2);
    }
}
