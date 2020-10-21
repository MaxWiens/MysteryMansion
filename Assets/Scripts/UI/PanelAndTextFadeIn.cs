using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelAndTextFadeIn : MonoBehaviour
{
    public void StartFade(GameObject overlay, Image overlayImage, TMP_Text[] texts)
    {
        Color c = overlayImage.color;
        c.a = 0;
        overlayImage.color = c;
        foreach (TMP_Text t in texts)
        {
            c = t.color;
            c.a = 0;
            t.color = c;
        }
        overlay.SetActive(true);
        StartCoroutine(DoFade(overlayImage, texts));
    }

    private IEnumerator DoFade(Image overlayImage, TMP_Text[] texts)
    {
        float time = 0;
        while (time < 2f)
        {
            time += Time.unscaledDeltaTime;
            time = Mathf.Min(time, 2f);
            Color c = overlayImage.color;
            c.a = 0.4f * time / 2f;
            overlayImage.color = c;
            foreach (TMP_Text t in texts)
            {
                c = t.color;
                c.a = time / 2f;
                t.color = c;
            }
            yield return null;
        }
    }
}
