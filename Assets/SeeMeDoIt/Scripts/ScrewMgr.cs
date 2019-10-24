using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrewMgr : MonoBehaviour
{
    GlobalsMgr g;
    const float nearDist = .1f;
    GameObject goBoltCurrent;
    GameObject goNutCurrent;
    float cntTurn;
    const float cntTurnMax = 50;
    bool ynTurn;
    float deltaDistY;
    float deltaTilt;
    GameObject goBoltLocked;

    private void Awake()
    {
        g = GetComponent<GlobalsMgr>();
    }

    // Update is called once per frame
    void Update()
    {
        if (g.mode == ModeType.place) return;
        if (g.gameType != GameType.Screw) return;
        UpdateAttach();
        UpdateTurn();
        UpdateProgressScrew();
        UpdateAuto();
    }

    void UpdateAuto() {
        if (g.ynAuto == true)
        {
            g.autoMgr.ResetTimeLastAuto();
            AutoScrew();
        }
    }

    void AutoScrew()
    {
        GameObject goBolt = g.screwMgr.FindNextBolt();
        if (goBolt != null)
        {
            GameObject goNut = g.screwMgr.FindNextNut();
            if (goNut != null)
            {
                g.assetMgr.MoveAssetToAsset(goBolt, goNut);
            }
            else
            {
                g.assetMgr.ButtonAdvanceAssetsClickedReLoad();
                Debug.Log("auto advance screw\n");
            }
        }
    }

    void UpdateProgressScrew()
    {
        if (g.progressMgr.numCompletedProgressLast != g.numCompletedProgress)
        {
            UpdateIndicatorsScrew();
            if (g.numCompletedProgress == g.numTotalProgress)
            {
                Invoke("CelebrateScrew", 1.5f);
            }
        }
    }

    void UpdateIndicatorsScrew()
    {
        Color colorOn = (Color.blue * 1.5f + Color.clear) / 2;
        Color colorOff = (Color.white * 1.5f + Color.clear) / 2;
        foreach (GameObject goList in g.goProgressLists)
        {
            for (int n = 0; n < goList.transform.childCount; n++)
            {
                GameObject go = goList.transform.GetChild(n).gameObject;
                if (n < g.numCompletedProgress)
                {
                    go.GetComponentInChildren<Renderer>().material.color = colorOn;
                }
                else
                {
                    go.GetComponentInChildren<Renderer>().material.color = colorOff;
                }
            }
        }
    }

    void CelebrateScrew()
    {
        //        Debug.Log("Celebrate Screw.................\n");
        g.assetMgr.ButtonAdvanceAssetsClickedReLoad();
    }

    public GameObject FindNextBolt()
    {
        List<GameObject> listPieces = g.assetMgr.GetListPieces();
        foreach (GameObject go in listPieces)
        {
            if (g.assetMgr.ContainsChildByName(go, "Bolt") == true)
            {
                return go;
            }
        }
        return null;
    }

    public GameObject FindNextNut()
    {
        List<GameObject> listPieces = g.assetMgr.GetListPieces();
        foreach (GameObject go in listPieces)
        {
            if (g.assetMgr.ContainsChildByName(go, "Nut") == true)
            {
                return go;
            }
        }
        return null;
    }

    void UpdateAttach()
    {
        if (ynTurn == true) return;
        UpdateAvailableNearBoltNut();
        if (goNutCurrent != null && goBoltCurrent != null)
        {
            Attach();
            g.ColorGo(goNutCurrent, Color.yellow);
            ynTurn = true;
            cntTurn = 0;
            deltaTilt = 4;
            float distY = goNutCurrent.transform.position.y - goBoltLocked.transform.position.y;
            deltaDistY = distY / cntTurnMax;
            g.numCompletedProgress++;
        }
    }

    void UpdateTurn()
    {
        if (ynTurn == false) return;
        if (cntTurn < cntTurnMax)
        {
            cntTurn++;
            Turn();
        }
        else
        {
            if (goNutCurrent != null)
            {
                g.ColorGo(goNutCurrent, Color.green);
            }
            ynTurn = false;
        }
    }

    void Turn()
    {
        if (goBoltLocked == null) return;
        Vector3 eulLocal = goBoltLocked.transform.localEulerAngles;
        eulLocal.z -= deltaTilt;
        goBoltLocked.transform.localEulerAngles = eulLocal;
        //
        Vector3 posLocal = goBoltLocked.transform.localPosition;
        posLocal.y += deltaDistY;
        goBoltLocked.transform.localPosition = posLocal;
    }

    void Attach()
    {
        g.RenameChild(goBoltCurrent, "Bolt", "BoltLocked");
        g.RenameChild(goNutCurrent, "Nut", "NutLocked");
        //
        GameObject goNutLocked = g.assetMgr.GetChildByName(goNutCurrent, "NutLocked");
        //
        goBoltCurrent.transform.position = goNutLocked.transform.position;
        //
        goBoltCurrent.transform.parent = goNutCurrent.transform;
        //
        string txt = goNutCurrent.name;
        g.assetMgr.SetAsset(goNutCurrent);
        goBoltCurrent = null;
        //
        g.assetMgr.AddHelpers();
        //
        g.smoothMgr.LoadLastsAndTargets();
        //
        goNutCurrent = g.assetMgr.GetChildByName(g.goAssets, txt);
        goBoltLocked = g.assetMgr.GetChildByName(goNutCurrent, "BoltLocked");
    }

    int GetAvailableBolts()
    {
        int cnt = 0;
        List<GameObject> listPieces = g.assetMgr.GetListPieces();
        foreach (GameObject go in listPieces)
        {
            if (g.assetMgr.ContainsChildByName(go, "Bolt") == true)
            {
                cnt++;
            }
        }
        return cnt;
    }

    int GetAvailableNuts()
    {
        int cnt = 0;
        List<GameObject> listPieces = g.assetMgr.GetListPieces();
        foreach (GameObject go in listPieces)
        {
            if (g.assetMgr.ContainsChildByName(go, "Nut") == true)
            {
                cnt++;
            }
        }
        return cnt;
    }

    public int GetNumTotalProgress()
    {
        int nBolts = GetAvailableBolts();
        int nNuts = GetAvailableNuts();
        if (nBolts < nNuts)
        {
            return nBolts;
        } else
        {
            return nNuts;
        }
    }

    void UpdateAvailableNearBoltNut()
    {
        goNutCurrent = null;
        goBoltCurrent = null;
        List<GameObject> listPieces = g.assetMgr.GetListPieces();
        foreach (GameObject go in listPieces)
        {
            if (g.assetMgr.ContainsChildByName(go, "Bolt") == true)
            {
                foreach (GameObject goCheck in listPieces)
                {
                    if (go != goCheck)
                    {
                        if (g.assetMgr.ContainsChildByName(goCheck, "Nut") == true)
                        {
                            if (g.IsNear(go, goCheck, nearDist) == true)
                            {
                                goBoltCurrent = go;
                                goNutCurrent = goCheck;
//                                Debug.Log("screw\n");
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
