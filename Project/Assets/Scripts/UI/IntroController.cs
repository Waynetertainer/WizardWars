using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

enum IntroState
{
    FadeIn,
    Show,
    FadeOut
};

public class IntroController : MonoBehaviour
{

    public float FadeTimer = 1f;
    public float ShowTimer = 2f;
    public List<SpriteRenderer> IntroScreens;
    public string NextSceneName = "MainMenu";

    private float currentTimer = 0;
    private int currentScreen = 0;
    private IntroState introState = IntroState.FadeIn;


    void Start()
    {
        foreach (SpriteRenderer sprite in IntroScreens) // everyting transparent
        {
            sprite.color = new Color(1, 1, 1, 0);

        }
    }

    void Update()
    {
        currentTimer += Time.deltaTime;
        switch (introState)
        {
            case IntroState.FadeIn:
                IntroScreens[currentScreen].color = new Color(1, 1, 1, currentTimer / FadeTimer);
                if (currentTimer >= FadeTimer)
                {
                    currentTimer = 0;
                    introState = IntroState.Show;
                    IntroScreens[currentScreen].color = new Color(1, 1, 1, 1);
                }

                break;
            case IntroState.Show:
                if (currentTimer >= ShowTimer)
                {
                    currentTimer = 0;
                    introState = IntroState.FadeOut;

                }
                break;
            case IntroState.FadeOut:

                if (currentTimer >= FadeTimer)
                {
                    ++currentScreen;
                    currentTimer = 0;
                    if (currentScreen < IntroScreens.Count)
                    {
                        IntroScreens[currentScreen - 1].color = new Color(1, 1, 1, 0);
                        introState = IntroState.FadeIn;
                        break;
                    }
                    else
                    {
                        Debug.Log("switching to " + NextSceneName);
                        SceneManager.LoadScene(NextSceneName);
                        break;

                    }
                }
                IntroScreens[currentScreen].color = new Color(1, 1, 1, 1 - (currentTimer / FadeTimer));
                break;

        }
        if (Input.anyKey)
        {
            ++currentScreen;
            if (currentScreen < IntroScreens.Count)
            {
                IntroScreens[currentScreen - 1].color = new Color(1, 1, 1, 0); // hide last screen
                currentTimer = 0;
                introState = IntroState.FadeIn;
            }
            else
            {
                SceneManager.LoadScene(NextSceneName);
            }
        }

    }
}
