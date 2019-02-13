
public enum eFactions
{
    Spawner,
    Player1,
    AI1,
    Player2,
    AI2
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
    EndTurn,
    EndOfMatch,
    AIturn,
    IdleLoop,
    InMenu

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
    Fight
};

public enum ePatrouilleSelection
{
    A,
    B,
    Static
};