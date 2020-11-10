using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public Slider volumeSlider;

    void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("volume", 0.75f);
    }

    public void SetVolume(float value)
    {
        PlayerPrefs.SetFloat("volume", value);
        AudioListener.volume = value;
    }
}
