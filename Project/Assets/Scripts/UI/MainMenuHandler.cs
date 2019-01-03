using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{

    public Color BackgroundFlickerLight;
    public Color BackgroundFlickerDark;
    public Image BackgroundImage;

    void FixedUpdate()
    {
        BackgroundImage.color = Color.Lerp(BackgroundFlickerLight, BackgroundFlickerDark, Random.value); // Spielerei :D
    }

    public void btnNewGame()
    {
        SceneManager.LoadScene("GD_Level01");
    }

    public void btnLoadGame()
    {
        //TODO: Load a saved game
    }

    public void btnOptions()
    {
        //TODO: Show options menu
    }

    public void btnCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void btnExit()
    {
        Application.Quit();
    }
}
