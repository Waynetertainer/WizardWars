using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum eFraction
{
    Gaia, Player1, AI1, Player2, AI2
}

public enum eGameState
{
    Select,
    Selected,
    Move,
    Moving,
    WaitForInput,
    FireSkill,
    FireUnique,
    Firing,
    End,
    EndOfMatch
}

public enum eVisibility
{
    Seethrough,
    Opaque
}

public enum eBlockType
{
    Empty,
    Blocked,
    HalfBlocked,
}

public enum eAIState
{
    Patrouille,
    Fight,
    Hunt
};