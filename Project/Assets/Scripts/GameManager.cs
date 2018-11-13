using System.Collections;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{

    public static GameManager pInstance;
    public eGameState pGameState;
    public System.Random pRandom = new System.Random();

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
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
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
                            if (pActiveCharacter.pReachableTiles.Contains(hit.transform.GetComponent<Tile>()))
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
            else if (pActiveCharacter != null)
            {
                pActiveCharacter.Deactivation();
                pGameState = eGameState.Select;
                OnGameStateChanged();
            }


            //    if (pActiveCharacter != null)
            //    {
            //        if (hit.transform.GetComponent<Character>() != null)
            //        {
            //            if (hit.transform != pActiveCharacter.transform)
            //            {
            //                pActiveCharacter.Deactivation();
            //                hit.transform.GetComponent<Character>().Activation();
            //            }
            //        }
            //        else
            //        {
            //            if (hit.transform.GetComponent<Tile>() != null)
            //            {
            //                if (pActiveCharacter.pReachableTiles.Contains(hit.transform.GetComponent<Tile>()))
            //                {
            //                    pActiveCharacter.Move(hit.transform.GetComponent<Tile>());
            //                }
            //                else
            //                {
            //                    pActiveCharacter.Deactivation();
            //                }
            //            }
            //            else
            //            {
            //                pActiveCharacter.Deactivation();
            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (hit.transform.GetComponent<Character>() != null)
            //        {
            //            hit.transform.GetComponent<Character>().Activation();
            //        }
            //    }
            //}
            //else
            //{
            //    if (pActiveCharacter != null)
            //    {
            //        pActiveCharacter.Deactivation();
            //    }
            //}
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

    private IEnumerator GameLoop()
    {
        yield return new WaitUntil(() => pGameState == eGameState.Move);

        yield return new WaitUntil(() => pGameState == eGameState.Moving);

        yield return new WaitUntil(() => pGameState == eGameState.Fire);

        yield return new WaitUntil(() => pGameState == eGameState.Firing);

        StartCoroutine(GameLoop());
    }
}
