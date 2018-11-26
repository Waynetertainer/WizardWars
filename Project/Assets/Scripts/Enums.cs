using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum eFraction
{
    Gaia, Player, PC
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
    End
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

