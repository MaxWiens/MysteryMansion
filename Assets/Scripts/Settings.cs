using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public List<QualityButton> qualityButtons;

    void Start()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("volume", 0.75f);
        CameraFollow.LookSpeed = PlayerPrefs.GetFloat("lookSensitivity", 0.35f);
        int quality = PlayerPrefs.GetInt("quality", 2);
        if (QualitySettings.GetQualityLevel() != quality)
        {
            QualitySettings.SetQualityLevel(quality, true);
        }

        foreach (QualityButton b in qualityButtons)
        {
            b.UpdateUI();
        }

        Destroy(gameObject);
    }
}
