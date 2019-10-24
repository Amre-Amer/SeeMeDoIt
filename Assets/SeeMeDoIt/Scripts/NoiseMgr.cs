using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMgr : MonoBehaviour
{
    GlobalsMgr g;
    AudioSource audioSourceCrash;
    AudioSource audioSourceKick;
    AudioSource audioSourceSnair;
    float timeStartCrash;
    float timeStartKick;
    float timeStartSnair;
    float delayCrash;
    float delayKick;
    float delaySnair;
    GameObject goCrash;
    GameObject goKick;
    GameObject goSnair;
    const float delayHighlight = .1f;
    AudioSource[] audioSources;
    List <GameObject> goSources = new List<GameObject>();
    List<float> timeStartHighlights = new List<float>();
    const float distNear = .2f;
    bool ynNear;
    bool ynNearLast;
    GameObject goTouch;
    GameObject goTouchLast;
    //
    GameObject goSourceCurrent;

    private void Awake()
    {
        g = GetComponent<GlobalsMgr>();
        delayCrash = 8f;
        delayKick = 1f;
        delaySnair = 4f;
        timeStartCrash = Time.realtimeSinceStartup;
        timeStartKick = Time.realtimeSinceStartup;
        timeStartSnair = Time.realtimeSinceStartup;
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSources = GetComponentsInChildren<AudioSource>();
        audioSourceCrash = audioSources[0];
        audioSourceKick = audioSources[1];
        audioSourceSnair = audioSources[2];
        timeStartHighlights.Add(Time.realtimeSinceStartup);
        timeStartHighlights.Add(Time.realtimeSinceStartup);
        timeStartHighlights.Add(Time.realtimeSinceStartup);
    }

    // Update is called once per frame
    void Update()
    {
        if (g.mode == ModeType.place) return;
        if (g.gameType != GameType.Noise) return;
        LoadSources();
        UpdateAccel();
        UpdateAuto();
        UpdateCamPlay();
        UpdateTouch();
        UpdateHighlights();
        goTouchLast = goTouch;
    }

    void UpdateAuto()
    {
        if (g.ynAuto == true)
        {
            g.autoMgr.ResetTimeLastAuto();
            AutoNoise();
        }
        AutoNoise();
    }

    void AutoNoise()
    {
        if (Time.realtimeSinceStartup - timeStartCrash > delayCrash)
        {
            timeStartCrash = Time.realtimeSinceStartup;
            Play(audioSourceCrash, goCrash);
            goSourceCurrent = goCrash;
            SendNoiseRemote();
        }
        if (Time.realtimeSinceStartup - timeStartKick > delayKick)
        {
            timeStartKick = Time.realtimeSinceStartup;
            Play(audioSourceKick, goKick);
            goSourceCurrent = goKick;
            SendNoiseRemote();
        }
        if (Time.realtimeSinceStartup - timeStartSnair > delaySnair)
        {
            timeStartSnair = Time.realtimeSinceStartup;
            Play(audioSourceSnair, goSnair);
            goSourceCurrent = goSnair;
            SendNoiseRemote();
        }
    }

    public void AddNoiseRemote(string txt)
    {
        if (g.gameType != GameType.Noise) return;
        UnSerializeNoise(txt);
    }

    GameObject GetGoSourceByName(string txt)
    {
        foreach(GameObject go in goSources)
        {
            if (go.name == txt)
            {
                return go;
            }
        }
        return null;
    }

    void UnSerializeNoise(string txt)
    {
        if (g.udpMgr.ExistsInReceives(txt) == true) return;  
        g.udpMgr.AddToReceives(txt);
        string[] stuff = txt.Split('@');
        GameObject go = GetGoSourceByName(stuff[0]);
        int n = goSources.IndexOf(go);
        if (n >= 0)
        {
            Play(audioSources[n], goSources[n]);
        }
    }

    void SendNoiseRemote()
    {
        string txtKey = "AddNoise";
        string txtValue = goSourceCurrent.name + "@" + Time.realtimeSinceStartup;
        string txt = g.udpMgr.CreateSendKeyValue(txtKey, txtValue);
        g.udpMgr.AddCreateSendKeyValueToTxtConfirmsExpires(txt);
    }

    void UpdateAccel()
    {
        int n = 0;
        if (goSourceCurrent != null)
        {
            n = goSources.IndexOf(goSourceCurrent);
        }
        if (Input.GetKey(KeyCode.Z) == true)
        {
            Play(audioSources[n], goSources[n]);
            g.autoMgr.ResetTimeLastTouch();
        }
        if (g.shakeMgr.ynShake != g.shakeMgr.ynShakeLast)
        {
            if (g.shakeMgr.ynShake == true)
            {
                Play(audioSources[n], goSources[n]);
                g.autoMgr.ResetTimeLastTouch();
                SendNoiseRemote();
                //                Handheld.Vibrate();
            }
        }
    }

    void UpdateTouch()
    {
        goTouch = null;
        if (g.touchMouseMgr.GetTouchMouseCount() == 0) return;
        RaycastHit hit = new RaycastHit();
        Vector3 scr = g.touchMouseMgr.GetTouchMouseScrPos();
        Ray ray = Camera.main.ScreenPointToRay(scr);
        Physics.Raycast(ray, out hit, 10, g.layerContent);
        if (hit.transform != null)
        {
            goTouch = g.assetMgr.FindAssetParent(hit.transform.gameObject);
            if (goTouch != null && goTouch != goTouchLast)
            {
                int n = goSources.IndexOf(goTouch);
                if (n >= 0)
                {
                    goSourceCurrent = goTouch;
                    Play(audioSources[n], goSources[n]);
                    g.autoMgr.ResetTimeLastTouch();
                    SendNoiseRemote();
                }
            }
        }
    }

    void UpdateCamPlay()
    {
        ynNear = false;
        for (int n = 0; n < audioSources.Length; n++)
        {
            ynNear = g.IsNear(g.camMain.gameObject, goSources[n], distNear);
            if (ynNear == true && ynNearLast == false)
            {
                Play(audioSources[n], goSources[n]);
                g.autoMgr.ResetTimeLastTouch();
                SendNoiseRemote();
            }
        }
        ynNearLast = ynNear;
    }

    void UpdateHighlights()
    {
        for(int n = 0; n < audioSources.Length; n++)
        {
            float t = timeStartHighlights[n];
            if (Time.realtimeSinceStartup - t > delayHighlight)
            {
                HighlightOnOff(goSources[n], false);
            }
        }
    }

    void Play(AudioSource audioSource, GameObject go)
    {
        audioSource.Play();
        int n = goSources.IndexOf(go);
        timeStartHighlights[n] = Time.realtimeSinceStartup;
        HighlightOnOff(go, true);
    }

    void LoadSources()
    {
        goSources.Clear();
        goCrash = g.assetMgr.GetChildByName(g.goAssets, "Woofer");
        goKick = g.assetMgr.GetChildByName(g.goAssets, "Midrange");
        goSnair = g.assetMgr.GetChildByName(g.goAssets, "Tweeter");
        goSources.Add(goCrash);
        goSources.Add(goKick);
        goSources.Add(goSnair);
    }

    void HighlightOnOff(GameObject go, bool yn)
    {
        float sca = 1.25f;
        if (yn == false)
        {
            sca = 1;
        }
        go.transform.localScale = Vector3.one * sca;
    }

    public int GetNumTotalProgress()
    {
        return 0;
    }
}
