using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextPlaceMgr : MonoBehaviour
{
    GlobalsMgr g;
    const float delay = 1f;
    float startTime;
    ModeType modeLast;

    private void Awake()
    {
        g = GetComponent<GlobalsMgr>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateModeChange();
        modeLast = g.mode;
    }

    void UpdateModeChange()
    {
        if (modeLast != g.mode)
        {
            if (g.mode == ModeType.place)
            {
                g.textPlace.gameObject.SetActive(true);
            }
            else
            {
                g.textPlace.gameObject.SetActive(false);
            }
        } else
        {
            if (g.mode == ModeType.place)
            {
                UpdateTextPlace();
            }
        }
    }

    void UpdateTextPlace()
    {
        if (Time.realtimeSinceStartup - startTime > delay)
        {
            startTime = Time.realtimeSinceStartup;
        }
        float factor = (Time.realtimeSinceStartup - startTime) / delay;

        float alpha = .25f + factor * .75f;
        ColorAlphaPlace(alpha);
    }

    void ColorAlphaPlace(float alpha)
    {
        Color color = g.textPlace.color;
        color = g.ColorAlpha(color, alpha);
        g.textPlace.color = color;
        //
        Image[] images = g.textPlace.GetComponentsInChildren<Image>();
        foreach (Image image in images)
        {
            color = image.color;
            color = g.ColorAlpha(color, alpha);
            image.color = color;
        }
    }
}
