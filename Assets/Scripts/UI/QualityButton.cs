using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QualityButton : MonoBehaviour
{
    public int level;
    public QualityButton other;
    [SerializeField]
    private Button button;

    private void Start()
    {
        UpdateUI();
    }

    public void OnClick()
    {
        if (QualitySettings.GetQualityLevel() != level)
        {
            QualitySettings.SetQualityLevel(level, true);
            UpdateUI();
            other.UpdateUI();
        }
    }

    public void UpdateUI()
    {
        button.interactable = QualitySettings.GetQualityLevel() != level;
    }
}
