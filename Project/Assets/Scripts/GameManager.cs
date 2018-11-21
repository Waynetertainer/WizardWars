using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager pInstance;
    public System.Random pRandom = new System.Random();

    public Character pActiveCharacter;

    [Header("Leveleditor On/Off")]
    public bool pEditmode;
    [Header("Level (Only for editor)")]
    public int pLevel;



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

    private void Update()
    {
        if (pEditmode)
        {

        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    if (pActiveCharacter != null)
                    {
                        if (hit.transform.GetComponent<Character>() != null)
                        {
                            if (hit.transform != pActiveCharacter.transform)
                            {
                                pActiveCharacter.Deactivation();
                                hit.transform.GetComponent<Character>().Activation();
                            }
                        }
                        else
                        {
                            if (hit.transform.GetComponent<Tile>() != null)
                            {
                                if (pActiveCharacter.pReachableTiles.Contains(hit.transform.GetComponent<Tile>()))
                                {
                                    pActiveCharacter.Move(hit.transform.GetComponent<Tile>());
                                }
                                else
                                {
                                    pActiveCharacter.Deactivation();
                                }
                            }
                            else
                            {
                                pActiveCharacter.Deactivation();
                            }
                        }
                    }
                    else
                    {
                        if (hit.transform.GetComponent<Character>() != null)
                        {
                            hit.transform.GetComponent<Character>().Activation();
                        }
                    }
                }
                else
                {
                    if (pActiveCharacter != null)
                    {
                        pActiveCharacter.Deactivation();
                    }
                }
            }
        }
    }
}
