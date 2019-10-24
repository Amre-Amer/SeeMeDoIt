using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongMgr : MonoBehaviour
{
    GlobalsMgr g;
    GameObject goPuck;
    GameObject goAvoid;
    const float nearDist = .1f;
    Vector3 vector;
    GameObject goBorder_L;
    GameObject goBorder_R;
    GameObject goBorder_U;
    GameObject goBorder_D;
    GameObject goPaddleRed;
    GameObject goPaddleBlue;
    const float delay = 3;
    const float randomVectorRange = .01f;
    const float vectorMagnitude = .125f;

    private void Awake()
    {
        g = GetComponent<GlobalsMgr>();
    }

    private void Start()
    {
        vector = Random.insideUnitCircle;
        vector.z = vector.y;
        vector.y = 0;
        vector *= vectorMagnitude;
        InvokeRepeating("ResetColors", .25f, .25f);
    }

    // Update is called once per frame
    void Update()
    {
        if (g.mode == ModeType.place) return;
        if (g.gameType != GameType.Pong) return;
        LoadPieces();
        UpdatePong();
        AutoPong();
        UpdateVector();
        UpdateAuto();
    }

    void UpdateAuto()
    {
        if (g.ynAuto == true)
        {
            g.autoMgr.ResetTimeLastAuto();
        }
    }

    public int GetNumTotalProgress()
    {
        return 0;
    }

    void RandomizeVector()
    {
        vector.x += Random.Range(-randomVectorRange, randomVectorRange);
//        vector = vector.normalized * vectorMagnitude;
    }

    public void AutoPong()
    {
        if (g.udpMgr.ynConnected == true) return;
        //if (g.ynAuto == false)
        //{
        if (g.goAsset == goPaddleRed)
        {
            AutoMovePaddle(goPaddleBlue);
        }
        else
        {
            if (g.goAsset == goPaddleBlue)
            {
                AutoMovePaddle(goPaddleRed);
            }
            else
            {
                //}
                //else
                //{
                //Debug.Log(".\n");
                AutoMovePaddle(goPaddleRed);
                AutoMovePaddle(goPaddleBlue);
            }
        }
        //}
    }

    void AutoMovePaddle(GameObject go)
    {
        Vector3 posLocalPuck = goPuck.transform.localPosition;
        Vector3 posLocal = go.transform.localPosition;
        posLocal.x = posLocalPuck.x;
        g.smoothMgr.UpdateAssetPosTarget(go, posLocal);
    }

    void UpdateVector()
    {
        RandomizeVector();
        Vector3 posTarget = goPuck.transform.localPosition + vector;
        g.smoothMgr.UpdateAssetPosTarget(goPuck, posTarget);
    }

    void LoadPieces()
    {
        goPuck = g.assetMgr.GetChildByName(g.goAssets, "Puck");
        goBorder_L = g.assetMgr.GetChildByName(g.goAssets, "Border_L");
        goBorder_R = g.assetMgr.GetChildByName(g.goAssets, "Border_R");
        goBorder_U = g.assetMgr.GetChildByName(g.goAssets, "Border_U");
        goBorder_D = g.assetMgr.GetChildByName(g.goAssets, "Border_D");
        goPaddleRed = g.assetMgr.GetChildByName(g.goAssets, "Paddle_Red");
        goPaddleBlue = g.assetMgr.GetChildByName(g.goAssets, "Paddle_Blue");
    }

    void UpdatePong()
    {
        bool yn = false;
        goAvoid = null;
        List<GameObject> gos = g.assetMgr.GetListPieces();
        foreach(GameObject go in gos)
        {
            if (g.assetMgr.IsPiece(go) == true)
            {
                if (go != goPuck)
                {
                    bool ynNear = false;
                    if (IsBorder(go) == true)
                    {
                        if (IsPuckNearBorder(go) == true)
                        {
                            ynNear = true;
                        }
                    } else
                    {
                        if (g.IsNear(go, goPuck, nearDist * 1.5f) == true)
                        {
                            ynNear = true;
                        }
                    }
                    if (ynNear == true)
                    {
                        goAvoid = go;
                        yn = true;
                        break;
                    }
                }
            }
        }
        if (yn == true)
        {
            g.ColorGo(goAvoid, (Color.green + Color.clear) / 2);
            if (IsBorder(goAvoid) == true)
            {
                AvoidBorder();
            }
            else
            {
                Avoid();
            }
            Invoke("ResetColors", .125f);
        }
    }

    void ResetColors()
    {
        if (g.gameType != GameType.Pong) return;
        if (g.mode == ModeType.place) return;
        LoadPieces();
        g.ColorGo(goBorder_L, Color.clear);
        g.ColorGo(goBorder_R, Color.clear);
        g.ColorGo(goBorder_U, Color.clear);
        g.ColorGo(goBorder_D, Color.clear);
    }

    bool IsBorder(GameObject go)
    {
        if (go.name.Contains("Border") == true)
        {
            return true;
        }
        return false;
    }

    void Avoid()
    {
        vector = goPuck.transform.localPosition - goAvoid.transform.localPosition;
    }

    void AvoidBorder()
    {
        if (goAvoid == goBorder_L)
        {
            vector.x = Mathf.Abs(vector.x);
            return;
        }
        if (goAvoid == goBorder_R) {
            vector.x = -Mathf.Abs(vector.x);
            return;
        }
        if (goAvoid == goBorder_U)
        {
            vector.z = -Mathf.Abs(vector.z);
            return;
        }
        if (goAvoid == goBorder_D) {
            vector.z = Mathf.Abs(vector.z);
            return;
        }
    }

    bool IsPuckNearBorder(GameObject go)
    {
        if(go == goBorder_L)
        {
            if (goPuck.transform.localPosition.x < go.transform.localPosition.x + nearDist)
            {
                return true;
            }
        }
        if (go == goBorder_R) {
            if (goPuck.transform.localPosition.x > go.transform.localPosition.x - nearDist)
            {
                return true;
            }
        }
        if (go == goBorder_U)
        {
            if (goPuck.transform.localPosition.z > go.transform.localPosition.z - nearDist)
            {
                return true;
            }
        }
        if (go == goBorder_D)
        {
            if (goPuck.transform.localPosition.z < go.transform.localPosition.z + nearDist)
            {
                return true;
            }
        }
        return false;
    }
}
