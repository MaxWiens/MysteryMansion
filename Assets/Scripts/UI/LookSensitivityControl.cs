using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LookSensitivityControl : MonoBehaviour
{
    public Slider lookSensitivitySlider;

    void Start()
    {
        lookSensitivitySlider.value = PlayerPrefs.GetFloat("lookSensitivity", 0.75f);
    }

    public void SetVolume(float value)
    {
        PlayerPrefs.SetFloat("lookSensitivity", value);
        CameraFollow.LookSpeed = value;
    }
}
