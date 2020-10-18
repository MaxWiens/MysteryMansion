using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeListener : MonoBehaviour
{
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
        pauseOverlay.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Input.Player.Menu.triggered && pauseOverlay != null && GameObject.FindGameObjectWithTag("Win Overlay") == null && GameObject.FindGameObjectWithTag("Lose Overlay") == null)
        {
            if (!pauseOverlay.activeSelf)
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
        }
    }
}
