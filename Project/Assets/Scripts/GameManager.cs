using System.Collections;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;
using NET_Multiplayer;

public class GameManager : MonoBehaviour
{

    public static GameManager pInstance;
    public GameObject pGridGameObject;
    public eGameState pGameState;
    public eFactions pCurrentFraction;

    public System.Random pRandom = new System.Random();
    public LayerMask RayCastLayers;

    public Character pActiveCharacter;

    public UIManager pUiManager;

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
        pUiManager.CloseAllWindows();

        EntityManager.pInstance.SpawnCharacters();

        EntityManager.pInstance.GetCharacterForId(0).Select();

        if (Random.Range(0, 2) == 0)
            pCurrentFraction = eFactions.AI1;
        else
            pCurrentFraction = eFactions.AI2;

        Debug.Log("Starting with " + pCurrentFraction.ToString());

        ChangeState(eGameState.AIturn);
    }

    private void Update()
    {
        if (pCurrentFraction == eFactions.Player1 || pCurrentFraction == eFactions.Player2)
        {
            CheckInput();
        }

    }

    private void CheckInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, RayCastLayers))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    return;

                switch (pGameState)
                {
                    case eGameState.Select:
                        if (hit.isType<Character>() && hit.transform.GetComponent<Character>().pFaction == pCurrentFraction)
                        {
                            if (EntityManager.pInstance.pGetFactionEntities(pCurrentFraction).Contains(hit.transform.GetComponent<Character>()))
                            {

                                hit.transform.GetComponent<Character>().Select();
                                ChangeState(eGameState.Move);
                            }
                            else
                            {
                                //TODO Show info of clicked character
                            }
                        }
                        break;
                    case eGameState.Selected:
                        if (hit.isType<Character>() && hit.transform.GetComponent<Character>().pFaction == pCurrentFraction)
                        {
                            pActiveCharacter.Deselect();
                            hit.transform.GetComponent<Character>().Select();
                            ChangeState(eGameState.Selected);
                        }
                        else
                        {
                            pActiveCharacter.Deselect();
                            ChangeState(eGameState.Select);
                        }
                        break;
                    case eGameState.Move:
                        if (hit.isType<Character>() && hit.transform.GetComponent<Character>().pFaction == pCurrentFraction)
                        {
                            if (pActiveCharacter.pApCurrent == pActiveCharacter.pAp
                                && EntityManager.pInstance.pGetFactionEntities(pCurrentFraction).Contains(hit.transform.GetComponent<Character>()))
                            {
                                pActiveCharacter.Deselect();
                                hit.transform.GetComponent<Character>().Select();
                                ChangeState(eGameState.Move);

                            }
                            else
                            {
                                //TODO Show info of clicked character
                            }
                        }
                        break;
                    case eGameState.FireSkill:
                        if (hit.isType<Tile>() && hit.transform.GetComponent<Tile>().pCharacterId != -1)
                        {
                            if (pActiveCharacter.pVisibleTiles.Contains(hit.transform.GetComponent<Tile>()))
                            {
                                ChangeState(eGameState.Firing);
                                pActiveCharacter.StandardAttack(hit.transform.GetComponent<Tile>());
                                if (pActiveCharacter.pApCurrent > 5)
                                {
                                    ChangeState(eGameState.FireSkill);
                                }
                                else
                                {
                                    ChangeState(eGameState.EndTurn);
                                }
                            }
                        }
                        else if (hit.isType<Character>())
                        {
                            if (pActiveCharacter.pVisibleTiles.Contains(hit.transform.GetComponent<Character>().pTile))
                            {
                                ChangeState(eGameState.Firing);
                                pActiveCharacter.StandardAttack(hit.transform.GetComponent<Character>().pTile);
                                if (pActiveCharacter.pApCurrent > 5)
                                {
                                    ChangeState(eGameState.FireSkill);
                                }
                                else
                                {
                                    ChangeState(eGameState.EndTurn);
                                }
                            }
                        }
                        else
                        {
                            ChangeState(eGameState.Move);
                        }
                        break;
                    case eGameState.FireUnique:
                        if (hit.isType<Tile>() && hit.transform.GetComponent<Tile>().pCharacterId != -1)
                        {
                            if (pActiveCharacter.pVisibleTiles.Contains(hit.transform.GetComponent<Tile>()))
                            {
                                ChangeState(eGameState.Firing);
                                pActiveCharacter.CastUnique(hit.transform.GetComponent<Tile>());
                            }
                        }
                        else if (hit.isType<Character>())
                        {
                            if (pActiveCharacter.pVisibleTiles.Contains(hit.transform.GetComponent<Character>().pTile))
                            {
                                ChangeState(eGameState.Firing);
                                pActiveCharacter.CastUnique(hit.transform.GetComponent<Character>().pTile);
                            }
                        }
                        else
                        {
                            ChangeState(eGameState.Move);
                        }
                        break;
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, RayCastLayers))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    return;

                switch (pGameState)
                {
                    case eGameState.Move:
                        if (hit.isType<Tile>())
                        {
                            if (pActiveCharacter.pReachableTiles.Contains(hit.transform.GetComponent<Tile>()))
                            {
                                pActiveCharacter.Move(hit.transform.GetComponent<Tile>());
                                ChangeState(eGameState.Moving);
                            }
                        }

                        break;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (pGameState)
            {
                case eGameState.Move:

                    break;
                case eGameState.FireSkill:
                    ChangeState(eGameState.Move);
                    break;
                case eGameState.FireUnique:
                    ChangeState(eGameState.Move);
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (pActiveCharacter != null)
                CameraMovement.SetTarget(pActiveCharacter.transform);
        }
    }

    public void ChangeState(eGameState state) //TODO: Replace with setter
    {
        Debug.Log("Changing Gamestate " + pGameState + " -> " + state + " faction: " + pCurrentFraction.ToString());

        if (pGameState == eGameState.EndOfMatch)
            return;

        pGameState = state;

        switch (pGameState)
        {
            case eGameState.Select:
                pGridGameObject.SetActive(false);
                if (pActiveCharacter != null)
                    pActiveCharacter.Deselect();
                break;
            case eGameState.Selected:
                pGridGameObject.SetActive(false);
                pActiveCharacter.HideRange();
                pActiveCharacter.HideView();
                break;
            case eGameState.Move:
                pActiveCharacter.HideView();
                pActiveCharacter.HideRange();
                pActiveCharacter.ShowRange();
                pGridGameObject.SetActive(true);
                break;
            case eGameState.Moving:
                break;
            case eGameState.WaitForInput:
                GridManager.pInstance.HidePath();
                pActiveCharacter.HideView();
                pActiveCharacter.HideRange();
                break;
            case eGameState.FireSkill:
                pActiveCharacter.HideRange();
                pActiveCharacter.HideView();
                pActiveCharacter.ShowView();
                pGridGameObject.SetActive(true);
                break;
            case eGameState.FireUnique:
                pActiveCharacter.HideRange();
                pActiveCharacter.HideView();
                pActiveCharacter.ShowView();
                pGridGameObject.SetActive(true);
                break;
            case eGameState.EndTurn:
                pActiveCharacter.HideRange();
                pActiveCharacter.HideView();
                pActiveCharacter.pAura.gameObject.SetActive(false);
                EntityManager.pInstance.EndRound(pActiveCharacter);
                pActiveCharacter.Deselect();

                pCurrentFraction = pCurrentFraction == eFactions.Player1 ? eFactions.AI2 : eFactions.AI1;


                ChangeState(eGameState.AIturn);
                break;
            case eGameState.AIturn:

                GameManager.pInstance.pCurrentFraction = GameManager.pInstance.pCurrentFraction == eFactions.AI1 ? eFactions.Player1 : eFactions.Player2;
                GameManager.pInstance.ChangeState(eGameState.Select);
                break;

                if (EntityManager.pInstance.pGetCurrentFactionEntities(pCurrentFraction).Count > 0) // skip AI turn if no AI present for faction
                {
                    for (int i = 0; i < 1; ++i) //HACK three ai moves per turn, debug only one
                    {
                        Character ai;
                        if (pCurrentFraction == eFactions.AI1)
                        {

                            if (EntityManager.pInstance.pGetCurrentFactionEntities(pCurrentFraction).Count >= EntityManager.pInstance.mAI1EntityPointer)
                                EntityManager.pInstance.mAI1EntityPointer = 0; // loop to first entity
                            ai = EntityManager.pInstance.pGetCurrentFactionEntities(pCurrentFraction)[EntityManager.pInstance.mAI1EntityPointer++];

                        }
                        else // faction AI2
                        {
                            if (EntityManager.pInstance.pGetCurrentFactionEntities(pCurrentFraction).Count >= EntityManager.pInstance.mAI2EntityPointer)
                                EntityManager.pInstance.mAI2EntityPointer = 0; // loop to first entity
                            ai = EntityManager.pInstance.pGetCurrentFactionEntities(pCurrentFraction)[EntityManager.pInstance.mAI2EntityPointer++];
                        }
                        CameraMovement.SetTarget(ai.transform);
                        StartCoroutine(AIevaluator.EvaluateAI(ai));
                    }
                }
                



                break;
        }
        pUiManager.Refresh();

    }
}