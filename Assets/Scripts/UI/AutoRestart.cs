using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoRestart : MonoBehaviour
{
    void Update()
    {
        if (InputManager.Input.Player.Interact.triggered)
        {
            Time.timeScale = 1;
            SceneManager.LoadSceneAsync("Scenes/MainMenu");
        }
    }
}
