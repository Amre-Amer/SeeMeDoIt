using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeMgr : MonoBehaviour
{
    GlobalsMgr g;
    ModeType modeLast;

    private void Awake()
    {
        g = GetComponent<GlobalsMgr>();
    }

    // Start is called before the first frame update
    void Start()
    {
        g.ynDetect = true;
        UpdateMode();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMode();
        modeLast = g.mode;
    }

    public void UpdateMode()
    {
        if (g.ynDetect == false && g.ynRotateGround == false)
        {
            g.mode = ModeType.play;
            //g.autoMgr.TurnOnOffAutoImages(true);
        }
        else
        {
            g.mode = ModeType.place;
        }
        if (modeLast != g.mode)
        {
            bool ynShowPlaneAndProtractor = g.mode == ModeType.place;
            g.imagePlane.gameObject.SetActive(ynShowPlaneAndProtractor);
            g.planeFinderMgr.PlaneFinderOnOff(ynShowPlaneAndProtractor);
            g.autoMgr.ResetTimeLastTouch();
            g.DebugLog("Mode " + g.mode);
        }
    }
}
