using UnityEngine;
using UnityEngine.UI;

public class AutoMgr : MonoBehaviour
{
    GlobalsMgr g;
    int touchCount;
    public Image imageAuto;
    public Image imageAdvance;
    bool ynAutoLast;

    private void Awake()
    {
        g = GetComponent<GlobalsMgr>();
        imageAuto = GameObject.Find("ImageAuto").GetComponent<Image>();
        imageAdvance = GameObject.Find("ImageAdvance").GetComponent<Image>();
    }

    private void Start()
    {
        //        ResetTimeLastAuto();
        TurnOnOffAutoImages(false);
    }

    private void Update()
    {
        if (g.mode == ModeType.place) return;
        if (g.role != RoleType.sender) return;
        touchCount = g.touchMouseMgr.GetTouchMouseCount();
        //UpdateAuto();
        //UpdateAutoAdvance();
        //UpdateImageAuto();
        //UpdateImageAutoAdvance();
        ynAutoLast = g.ynAuto;
    }

    private void UpdateAuto()
    {
        UpdateYnAuto();
        //Debug.Log("auto " + g.ynAuto + "\n");
        if (g.ynAuto != ynAutoLast)
        {
            //TurnOnOffAutoImages(g.ynAuto);
//            Debug.Log("ynAuto " + g.ynAuto + "\n");
        }
    }

    void UpdateYnAuto()
    {
       //g.ynAuto = false;
        if (touchCount > 0)
        {
            ResetTimeLastTouch();
            return;
        }
//        Debug.Log("diff " + (Time.realtimeSinceStartup - g.timeLastTouchAuto) + "\n");
        if (Time.realtimeSinceStartup - g.timeLastTouchAuto > g.delayAuto)
        {
            //g.ynAuto = true;
        }
    }

    private void UpdateAutoAdvance()
    {
        if (Time.realtimeSinceStartup - g.timeLastTouchAutoAdvance > g.delayAutoAdvance)
        {
            Debug.Log("UpdateAutoAdvance................................\n");
            ResetTimeLastAutoAdvance();
            g.assetMgr.ButtonAdvanceAssetsClicked();
        }
    }

    void UpdateImageAuto()
    {
        float factor = (Time.realtimeSinceStartup - g.timeLastTouchAuto) / g.delayAuto;
        float sca = .25f + factor * .75f;
        imageAuto.transform.localScale = Vector3.one * sca;
    }

    void UpdateImageAutoAdvance()
    {
        float factor = (Time.realtimeSinceStartup - g.timeLastTouchAutoAdvance) / g.delayAutoAdvance;
        float sca = .25f + factor * .75f;
        imageAdvance.transform.localScale = Vector3.one * sca;
    }

    public void TurnOnOffAutoImages(bool yn)
    {
        imageAuto.gameObject.SetActive(yn);
        imageAdvance.gameObject.SetActive(yn);
//        ResetTimeLastAuto();
    }

    public void ResetTimeLastTouch()
    {
//        Debug.Log("ResetTimeLastTouch\n");
        g.timeLastTouch = Time.realtimeSinceStartup;
        g.timeLastTouchAuto = Time.realtimeSinceStartup;
        g.timeLastTouchAutoAdvance = Time.realtimeSinceStartup;
        g.delayAuto = g.delayAutoFirst;
        g.ynAuto = false;
    }

    public void ResetTimeLastAutoAdvance()
    {
//        Debug.Log("ResetTimeLastAutoAdvance\n");
        g.timeLastTouchAutoAdvance = Time.realtimeSinceStartup;
        g.delayAuto = g.delayAutoOrig;
        g.ynAuto = false;
    }

    public void ResetTimeLastAuto()
    {
//        Debug.Log("ResetTimeLastAuto\n");
        g.timeLastTouchAuto = Time.realtimeSinceStartup;
        g.delayAuto = g.delayAutoOrig;
        g.ynAuto = false;
    }
}
