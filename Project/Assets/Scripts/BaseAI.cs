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
    static void Move(Character c)
    {
        List<Tile> reachableTiles = GridManager.pInstance.GetReachableTiles(c.pTile, c.pAp < c.pWalkRange ? c.pAp : c.pWalkRange);
        reachableTiles = reachableTiles.FindAll(T => T.pCharacterId == -1);
        Tile target = reachableTiles[Random.Range(0, reachableTiles.Count)];

        c.transform.position = target.transform.position;
        c.transform.position += Vector3.up;
        c.pTile.pCharacterId = -1;
        c.pAp -= Tile.Distance(c.pTile, target);
        c.pTile = target;
        target.pCharacterId = EntityManager.pInstance.GetIdForCharacter(c);
    }

    public static IEnumerator AIBehaviour(Character c)
    {
        yield return new WaitForSeconds(1);
        Move(c);
        yield return new WaitForSeconds(1);
        GameManager.pInstance.ChangeState(eGameState.Select);
    }
}
