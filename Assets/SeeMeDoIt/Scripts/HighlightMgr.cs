using UnityEngine;

public class HighlightMgr : MonoBehaviour
{
    GlobalsMgr g;
    bool ynHighlight;
    public GameObject goHighlight;
    public GameObject goHighlightLast;
    const float delay = .1f;
    const float timeLimit = .75f;
    float timeStart;
    GameObject goAssetLast;

    public void Awake()
    {
        g = GetComponent<GlobalsMgr>();
    }

    private void Start()
    {
        //InvokeRepeating("UpdateHighlight", delay, delay);
    }

    //  Update like
    private void UpdateHighlight() // not every frame (delay)
    {
        if (g.mode == ModeType.place) return;
        UpdateHighlightAsset();
        UpdateHighlightFlash();
        goHighlightLast = goHighlight;
        goAssetLast = g.goAsset;
    }

    // Update is called once per frame
    void UpdateHighlightAsset()
    {
        if (g.goAsset == null) return;
        if (g.goAsset != goAssetLast)
        {
            goHighlight = g.goAsset;
        }
        g.goAsset.gameObject.SetActive(true);
    }

    public void UpdateHighlightFlash()
    {
        if (goHighlight == null) return;
        if (goHighlight != goHighlightLast)
        {
            if (goHighlightLast != null)
            {
                Highlight(goHighlightLast, true);
            }
            ynHighlight = true;
            Highlight(goHighlight, true);
            timeStart = Time.realtimeSinceStartup;
            g.ynShowBoundingBox = true;
        }
        else
        {
            if (Time.realtimeSinceStartup - timeStart < timeLimit)
            {
                ynHighlight = !ynHighlight;
                Highlight(goHighlight, ynHighlight);
            }
            else
            {
                ynHighlight = true;
                Highlight(goHighlight, true);
                g.ynShowBoundingBox = false;
            }
        }
    }

    void UnHighlight()
    {
        Highlight(goHighlight, false);
    }

    void Highlight(GameObject go, bool yn)
    {
        if (go.activeSelf != yn)
        {
            go.SetActive(yn);
        }
    }
}
