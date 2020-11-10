using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    void Start()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("volume", 0.75f);
        CameraFollow.LookSpeed = PlayerPrefs.GetFloat("lookSensitivity", 0.35f);
        Destroy(gameObject);
    }
}
