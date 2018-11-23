using System.Collections;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{

    public static GameManager pInstance;
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
        pGameState = eGameState.Select;
        OnGameStateChanged();
    }

    private void Update()
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
                    if (hit.transform.GetComponent<Character>() != null)
                    {
                        hit.transform.GetComponent<Character>().Activation();
                        pGameState = eGameState.Selected;
                        OnGameStateChanged();
                    }
                    break;
                case eGameState.Selected:
                    if (hit.transform.GetComponent<Character>() != null)
                    {
                        pActiveCharacter.Deactivation();
                        hit.transform.GetComponent<Character>().Activation();
                    }
                    else
                    {
                        pActiveCharacter.Deactivation();
                        pGameState = eGameState.Select;
                        OnGameStateChanged();
                    }
                    break;
                case eGameState.Move:
                    if (hit.transform.GetComponent<Tile>() != null)
                    {
                        if (hit.transform.GetComponent<Tile>() != null)
                        {
                            pActiveCharacter.Move(hit.transform.GetComponent<Tile>());
                            pGameState = eGameState.Fire;
                            OnGameStateChanged();
                        }
                        else
                        {
                            pActiveCharacter.Deactivation();
                        }
                    }

                    break;
                case eGameState.Moving:
                    break;
                case eGameState.Fire:
                    break;
                case eGameState.Firing:
                    break;
            }
        }
        else
        {
            if (pActiveCharacter != null)
            {
                pActiveCharacter.Deactivation();
                pGameState = eGameState.Select;
                OnGameStateChanged();
            }
        }
    }

    private void OnGameStateChanged()
    {
        switch (pGameState)
        {
            case eGameState.Select:
                pUiManager.CloseAllWindos();
                break;
            case eGameState.Selected:
                pUiManager.ShowSelectionScreen();
                pGameState = eGameState.Move;
                break;
            case eGameState.Move:
                break;
            case eGameState.Moving:
                break;
            case eGameState.Fire:
                break;
            case eGameState.Firing:
                break;
        }
    }
}