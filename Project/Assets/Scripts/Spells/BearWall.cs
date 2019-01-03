/*
* Copyright (c) Jannik Lietz
* http://www.janniklietz.wordpress.com
*/

using UnityEngine;

[CreateAssetMenu]
public class BearWall : ScriptableObject, IUniqueSpell
{
    [SerializeField] private string _SpellName;
    [SerializeField] private int _Damage;
    [SerializeField] private int _Cost;
    [SerializeField] private int _Range;

    public string SpellName
    {
        get { return _SpellName; }
    }
    public int Damage
    {
        get { return _Damage; }
    }
    public int Cost
    {
        get { return _Cost; }
    }
    public int Range
    {
        get { return _Range; }
    }

    private Vector2Int mOffsetLeft = new Vector2Int(1, 1);
    private Vector2Int mOffsetRight = new Vector2Int(1, -1);

    private Tile mRight;
    private Tile mLeft;

    public void CastUnique(Tile t)
    {
        mRight = GridManager.pInstance.GetTileAt(t.pPosition + mOffsetRight);
        mLeft = GridManager.pInstance.GetTileAt(t.pPosition + mOffsetLeft);

        t.pBlockType = eBlockType.Blocked;
        mLeft.pBlockType = eBlockType.HalfBlocked;
        mRight.pBlockType = eBlockType.HalfBlocked;
    }

    public void ShowUniquePreview(Tile t)
    {
        mLeft = GridManager.pInstance.GetTileAt(t.pPosition + mOffsetLeft);
        mRight = GridManager.pInstance.GetTileAt(t.pPosition + mOffsetRight);

        t.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
        mLeft.GetComponent<Renderer>().material.SetColor("_Color", Color.cyan);
        mRight.GetComponent<Renderer>().material.SetColor("_Color", Color.cyan);
    }

    public void HideUniquePreview(Tile t)
    {
        t.GetComponent<Renderer>().material.SetColor("_Color", t.Color);
        mLeft.GetComponent<Renderer>().material.SetColor("_Color", mLeft.Color);
        mRight.GetComponent<Renderer>().material.SetColor("_Color", mRight.Color);
    }
}
