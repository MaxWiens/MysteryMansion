using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeListener : MonoBehaviour
{
    private bool paused;
    [SerializeField]
    private GameObject pauseOverlay;
    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.FindGameObjectWithTag("EscapeListener").GetComponent<EscapeListener>() != this)
        {
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(this);
        paused = false;
        pauseOverlay.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Game.Input.Player.Menu.triggered && pauseOverlay != null)
        {
            if (!paused)
            {
                Time.timeScale = 0;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                pauseOverlay.SetActive(true);
            }
            else
            {
                Time.timeScale = 1;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                pauseOverlay.SetActive(false);
            }
            paused = !paused;
        }
    }
}
