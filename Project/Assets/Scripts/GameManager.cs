using System.Collections;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{

    public static GameManager pInstance;
    public GameObject pGridGameObject;
    public eGameState pGameState;
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

        EntityManager.pInstance.SpawnCharacters(
            GridManager.pInstance.pCurrentLevel.pPlayerSpawns,
            GridManager.pInstance.pCurrentLevel.pPCSpawns);

        EntityManager.pInstance.GetCharacterForId(0).Select();
        ChangeState(eGameState.Selected);
    }

    private void Update()
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
                        if (hit.transform.GetComponent<Character>() != null && hit.transform.GetComponent<Character>().pFraction == eFraction.Player)
                        {
                            hit.transform.GetComponent<Character>().Select();
                            ChangeState(eGameState.Selected);
                        }
                        break;
                    case eGameState.Selected:
                        if (hit.transform.GetComponent<Character>() != null && hit.transform.GetComponent<Character>().pFraction == eFraction.Player)
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
                        if (hit.transform.GetComponent<Tile>() != null)
                        {
                            if (pActiveCharacter.pReachableTiles.Contains(hit.transform.GetComponent<Tile>()))
                            {
                                pActiveCharacter.Move(hit.transform.GetComponent<Tile>());
                                ChangeState(eGameState.Move);
                            }
                        }
                        else if (hit.transform.GetComponent<Character>() != null && hit.transform.GetComponent<Character>().pFraction == eFraction.Player)
                        {
                            pActiveCharacter.Deselect();
                            hit.transform.GetComponent<Character>().Select();
                            ChangeState(eGameState.Selected);
                        }
                        break;
                    case eGameState.Moving:
                        break;
                    case eGameState.WaitForInput:
                        break;
                    case eGameState.FireSkill:
                        if (hit.transform.GetComponent<Tile>() != null)
                        {
                            if (pActiveCharacter.pVisibleTiles.Contains(hit.transform.GetComponent<Tile>()))
                            {
                                pActiveCharacter.StandardAttack(hit.transform.GetComponent<Tile>());
                                ChangeState(eGameState.WaitForInput);
                            }
                        }
                        else if (hit.transform.GetComponent<Character>() != null)
                        {
                            if (pActiveCharacter.pVisibleTiles.Contains(hit.transform.GetComponent<Character>().pTile))
                            {
                                pActiveCharacter.StandardAttack(hit.transform.GetComponent<Character>().pTile);
                                ChangeState(eGameState.WaitForInput);
                            }
                        }
                        break;
                    case eGameState.FireUnique:
                        if (hit.transform.GetComponent<Tile>() != null)
                        {
                            if (pActiveCharacter.pVisibleTiles.Contains(hit.transform.GetComponent<Tile>()))
                            {
                                pActiveCharacter.CastUnique(hit.transform.GetComponent<Tile>());
                                ChangeState(eGameState.WaitForInput);
                            }
                        }
                        break;
                }
            }
        }
    }

    public void ChangeState(eGameState state)
    {
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
                pActiveCharacter.HideView();
                pActiveCharacter.HideRange();
                break;
            case eGameState.FireSkill:
                pActiveCharacter.HideRange();
                pActiveCharacter.ShowView();
                pGridGameObject.SetActive(true);
                break;
            case eGameState.FireUnique:
                pActiveCharacter.HideRange();
                pActiveCharacter.ShowView();
                pGridGameObject.SetActive(true);
                break;
            case eGameState.End:
                EntityManager.pInstance.EndRound(pActiveCharacter);
                pActiveCharacter.Deselect();

                Character ai = EntityManager.pInstance.pCurrentPCPlayers[
                    Random.Range(0, EntityManager.pInstance.pCurrentPCPlayers.Count)];
                StartCoroutine(BaseAI.AIBehaviour(ai));

                EntityManager.pInstance.EndRound(ai);
                break;
        }
        pUiManager.Refresh();

    }
}