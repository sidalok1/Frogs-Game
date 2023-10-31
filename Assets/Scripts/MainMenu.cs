using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnPlayButton () { // goes into game
        SceneManager.LoadScene(1); // loads second scene
    }

    public void OnControlsButton () { // shows how to use controls to play

    }

    public void OnQuitButton () { // closes the game
        Application.Quit();
    }
}
