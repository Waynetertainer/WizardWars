using System.Collections;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

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
        FindObjectsOfType<Character>()[0].Select();
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
                                ChangeState(eGameState.WaitForInput);
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
                                pActiveCharacter.StandardAttack();
                                ChangeState(eGameState.WaitForInput);
                            }
                        }
                        break;
                    case eGameState.FireUnique:
                        if (hit.transform.GetComponent<Tile>() != null)
                        {
                            if (pActiveCharacter.pVisibleTiles.Contains(hit.transform.GetComponent<Tile>()))
                            {
                                pActiveCharacter.UniqueAttack();
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
                ChangeState(eGameState.Select);
                break;
        }
        pUiManager.Refresh();

    }
}