using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VictoryOverlayController : PanelAndTextFadeIn
{
    public void DoVictory()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        GameObject.FindGameObjectWithTag("Play Panel").SetActive(false);

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).CompareTag("Win Overlay"))
            {
                GameObject victoryOverlay = transform.GetChild(i).gameObject;
                TMP_Text[] texts = victoryOverlay.GetComponentsInChildren<TMP_Text>();
                Image image = victoryOverlay.GetComponent<Image>();
                StartFade(victoryOverlay, image, texts);
                break;
            }
        }
    }
}
