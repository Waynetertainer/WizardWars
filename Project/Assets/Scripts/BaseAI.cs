/*
* Copyright (c) Jannik Lietz
* http://www.janniklietz.wordpress.com
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseAI
{
    public static IEnumerator AIBehaviour(Character c)
    {
        yield return new WaitForSeconds(1);
        List<Tile> reachableTiles = GridManager.pInstance.GetReachableTiles(c.pTile, c.pWalkRange);
        reachableTiles = reachableTiles.FindAll(T => T.pCharacterId == -1);
        Tile target = reachableTiles[Random.Range(0, reachableTiles.Count)];

        List<Tile> path = GridManager.pInstance.GetPathTo(c.pTile, target);

        for (int i = path.Count - 1; i >= 0; i--)
        {
            var tile = path[i];

            while (Vector3.Distance(c.transform.position, tile.transform.position) >= 0.1f)
            {
                c.transform.position = Vector3.Lerp(c.transform.position, tile.transform.position, Time.deltaTime * 5);
                yield return null;
            }
            yield return new WaitForSeconds(0.2f);
        }

        c.pTile.pCharacterId = -1;
        c.pApCurrent -= Tile.Distance(c.pTile, target);
        c.pTile = target;
        target.pCharacterId = EntityManager.pInstance.GetIdForCharacter(c);
        yield return new WaitForSeconds(1);
        EntityManager.pInstance.pCurrentPlayers[Random.Range(0, EntityManager.pInstance.pCurrentPlayers.Count)].Select();
        GameManager.pInstance.ChangeState(eGameState.Selected);
    }
}
