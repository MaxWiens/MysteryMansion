using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QualityButton : MonoBehaviour
{
    public int level;

    public void OnClick()
    {
        if (QualitySettings.GetQualityLevel() != level)
        {
            QualitySettings.SetQualityLevel(level, true);
        }
    }
}
