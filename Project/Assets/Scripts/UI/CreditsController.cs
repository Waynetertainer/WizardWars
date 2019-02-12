using UnityEngine;

public class CreditsController : MonoBehaviour {

    void Update () 
    {
        if (Input.anyKey && Time.timeSinceLevelLoad > 1f)
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
	}
}
