using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressMgr : MonoBehaviour
{
    GlobalsMgr g;
    public int numCompletedProgressLastX;
    public int numCompletedProgressLastO;
    public int numCompletedProgressLast;
    Image imageProgress;
    float fTarget;
    const float smooth = .1f;

    private void Awake()
    {
        g = GetComponent<GlobalsMgr>();
        imageProgress = GameObject.Find("ImageProgress").GetComponent<Image>();
    }

    private void Start()
    {
        g.numTotalProgress = 5;
        CreateProgress();
    }

    // Update is called once per frame
    void Update()
    {
        if (g.mode == ModeType.place) return;
        UpdateProgress();
        numCompletedProgressLastX = g.numCompletedProgressX;
        numCompletedProgressLastO = g.numCompletedProgressO;
        numCompletedProgressLast = g.numCompletedProgress;
    }

    void UpdateProgress()
    {
        if (imageProgress.enabled == false) return;
        float f = smooth * fTarget + (1 - smooth) * imageProgress.transform.localScale.x;
        imageProgress.transform.localScale = Vector3.one * f;
        Color color = f * Color.green + (1 - f) * Color.red;
        imageProgress.color = color;
    }

    public void UpdateImageProgress(float fract)
    {
        if (fract > 1)
        {
            Invoke("HideImageProgress", 1);
            return;
        }
        if (fract < 0)
        {
            imageProgress.transform.localScale = Vector3.zero;
        }
        imageProgress.enabled = true;
        fTarget = .1f + fract * .9f;
    }

    public void HideImageProgress()
    {
        imageProgress.enabled = false;
    }

    void SwitchSides()
    {
        SwitchSidesPieces();
    }

    void SwitchSidesPieces()
    {
        for(int n = 0; n < g.goAssets.transform.childCount; n++)
        {
            GameObject go = g.goAssets.transform.GetChild(n).gameObject;
            Vector3 posLocal = g.smoothMgr.posTargets[n];
            posLocal.x *= -1;
            g.smoothMgr.UpdateAssetPosTarget(go, posLocal);
        }
    }

    public SolutionItemType GetProgressListType(GameObject go)
    {
        SolutionItemType solutionItemType = SolutionItemType.empty;
        if (go.name == "ProgressListX")
        {
            solutionItemType = SolutionItemType.X;
        }
        if (go.name == "ProgressListO")
        {
            solutionItemType = SolutionItemType.O;
        }
        return solutionItemType;
    }

    void UpdateNumTotalProgress()
    {
        switch (g.gameType)
        {
            case GameType.TicTacToe:
                g.numTotalProgress = g.ticTacToeMgr.GetNumTotalProgress();
                break;
            case GameType.Connect:
                g.numTotalProgress = g.connectMgr.GetNumTotalProgress();
                break;
            case GameType.Screw:
                g.numTotalProgress = g.screwMgr.GetNumTotalProgress();
                break;
            case GameType.Chess:
                g.numTotalProgress = g.chessMgr.GetNumTotalProgress();
                break;
            case GameType.Pong:
                g.numTotalProgress = g.pongMgr.GetNumTotalProgress();
                break;
            case GameType.Sculpt:
                g.numTotalProgress = g.sculptMgr.GetNumTotalProgress();
                break;
            case GameType.Noise:
                g.numTotalProgress = g.noiseMgr.GetNumTotalProgress();
                break;
        }
    }

    public void CreateProgress()
    {
//        Debug.Log("CreateProgress\n");
        UpdateNumTotalProgress();
        CleanProgress();
        g.goProgressIndicator.SetActive(true);
        foreach (GameObject goList in g.goProgressLists)
        {
            for (int n = 0; n < g.numTotalProgress; n++)
            {
                GameObject go = Instantiate(g.goProgressIndicator, goList.transform);
                float x = n * .075f;
                go.transform.localPosition = new Vector3(x, 0, 0);
            }
        }
        g.goProgressIndicator.SetActive(false);
        g.numCompletedProgressX = 0;
        g.numCompletedProgressO = 0;
        g.numCompletedProgress = 0;
    }

    void CleanProgress()
    { 
        foreach(GameObject go in g.goProgressLists) {
            for(int n = 0; n < go.transform.childCount; n++)
            {
                Destroy(go.transform.GetChild(n).gameObject);
            }
        }
    }
}
