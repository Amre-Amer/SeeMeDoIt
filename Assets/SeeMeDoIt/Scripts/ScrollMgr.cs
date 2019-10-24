using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollMgr : MonoBehaviour
{
    GlobalsMgr g;
    int touchCount;
    int touchCountLast;
    const float smooth = .1f;
    const float factor = .1f;
    Vector3 scrScroll;
    float posLimitMinY;
    float posLimitMaxY;
    float posTargetY;
    Vector3 posScroll;
    ScreenOrientation screenOrientationLast;
    int cnt;

    private void Awake()
    {
        g = GetComponent<GlobalsMgr>();
    }

    // Start is called before the first frame update
    void Start()
    {
        float ht = g.imageInfo.GetComponent<RectTransform>().sizeDelta.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (g.ynInfo == false) return;
        touchCount = g.touchMouseMgr.GetTouchMouseCount();
        UpdateImageInfoScale();
        UpdateScroll();
        Limit();
        Smooth();
        touchCountLast = g.touchMouseMgr.GetTouchMouseCount();
        screenOrientationLast = Screen.orientation;
    }

    void UpdateImageInfoScale()
    {
        if (Screen.orientation == screenOrientationLast) return;
        Debug.Log("UpdateImageInfoScale " + cnt);
        Image img = g.imageInfo.GetComponent<Image>();
        float fract = (float)img.mainTexture.height / img.mainTexture.width;
        //
        Rect rect = g.imageInfo.transform.GetComponent<RectTransform>().rect;
        rect.width = Screen.width;
        rect = new Rect(rect.center, new Vector2(rect.width, rect.width * fract));
        //
        rect.Set(rect.x, rect.y, rect.width, rect.height);
        Vector2 sd = g.imageInfo.transform.GetComponent<RectTransform>().sizeDelta;
        sd.y = rect.height;
        g.imageInfo.transform.GetComponent<RectTransform>().sizeDelta = sd;
        //
        float dHeight = g.imageInfo.GetComponent<RectTransform>().rect.size.y - Screen.height;
        posLimitMinY = -dHeight / 2;
        posLimitMaxY = dHeight / 2;
        if (cnt == 0)
        {
            posTargetY = posLimitMinY;
        }
        cnt++;
    }

    void UpdateScroll()
    {
        if (touchCountLast != touchCount)
        {
            if (touchCount == 1)
            {
                scrScroll = g.touchMouseMgr.GetTouchMouseScrPos();
                posScroll = g.imageInfo.transform.localPosition;
            }
        }
        {
            if (touchCount == 1)
            {
                Vector3 scr = g.touchMouseMgr.GetTouchMouseScrPos();
                float dY = scr.y - scrScroll.y;
                Vector3 pos = posScroll + Vector3.up * dY; // * factor;
                posTargetY = pos.y;
            }
        }
    }

    void Smooth()
    {
        Vector3 pos = g.imageInfo.transform.localPosition;
        pos.y = smooth * posTargetY + (1 - smooth) * pos.y;
        g.imageInfo.transform.localPosition = pos;
    }

    void Limit()
    {
        if (posTargetY < posLimitMinY)
        {
            posTargetY = posLimitMinY;
        }
        if (posTargetY > posLimitMaxY)
        {
            posTargetY = posLimitMaxY;
        }
    }
}
