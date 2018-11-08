
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
}
