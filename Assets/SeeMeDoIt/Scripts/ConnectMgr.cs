using System.Collections.Generic;
using UnityEngine;

public class ConnectMgr : MonoBehaviour
{
    GlobalsMgr g;
    const float nearDist = .2f;
    GameObject goKeyHoleCurrent;
    GameObject goKeyCurrent;

    private void Awake()
    {
        g = GetComponent<GlobalsMgr>();
    }

    // Update is called once per frame
    void Update()
    {
        if (g.mode == ModeType.place) return;
        if (g.gameType != GameType.Connect) return;
        UpdateInteract();
        UpdateProgressConnect();
        UpdateAuto();
    }

    public int GetNumTotalProgress()
    {
        return GetKeyCount(g.goAssets);
    }

    void UpdateAuto()
    {
        if (g.ynAuto == true)
        {
            g.autoMgr.ResetTimeLastAuto();
            AutoConnect();
        }
    }

    public void AutoConnect()
    {
        GameObject goKey = FindNextKey();
        if (goKey != null)
        {
            GameObject goKeyHole = FindNextKeyHole();
            if (goKeyHole != null)
            {
                g.assetMgr.MoveAssetToAsset(goKey, goKeyHole);
            }
        }
    }

    void UpdateProgressConnect()
    {
        if (g.progressMgr.numCompletedProgressLast != g.numCompletedProgress)
        {
            UpdateIndicatorsConnect();
            if (g.numCompletedProgress == g.numTotalProgress)
            {
                Invoke("CelebrateConnect", 1.5f);
            }
        }
    }

    void CelebrateConnect()
    {
        Debug.Log("Celebrate Connect.................\n");
        g.assetMgr.ButtonAdvanceAssetsClickedReLoad();
    }

    void UpdateIndicatorsConnect()
    {
        Color colorOn = (Color.green * 1.5f + Color.clear) / 2;
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

    void UpdateInteract()
    {
        UpdateAssemble();
        Assemble();
    }

    public GameObject FindNextKey()
    {
        foreach(Transform t in g.goAssets.transform)
        {
            if (HasKeyAvailable(t.gameObject) == true)
            {
                return t.gameObject;
            }
        }
        return null;
    }

    public GameObject FindNextKeyHole()
    {
        foreach (Transform t in g.goAssets.transform)
        {
            if (HasKeyHoleAvailable(t.gameObject) == true)
            {
                return t.gameObject;
            }
        }
        return null;
    }

    void UpdateAssemble()
    {
        g.ynAssemble = false;
        if (IsAnyAvailableKeyNearAvailableKeyHole() == true)
        {
            g.ynAssemble = true;
        }
    }

    bool IsAnyAvailableKeyNearAvailableKeyHole()
    {
        foreach(Transform t in g.goAssets.transform)
        {
            if (HasKeyAvailable(t.gameObject) == true)
            {
                foreach(Transform tt in g.goAssets.transform)
                {
                    if (t != tt)
                    {
                        if (HasKeyHoleAvailable(tt.gameObject) == true)
                        {
                            if (IsNear(t.gameObject, tt.gameObject) == true)
                            {
                                goKeyCurrent = t.gameObject;
                                goKeyHoleCurrent = tt.gameObject;
//                                Debug.Log("Key and KeyHole Available\n");
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    bool HasKeyHoleAvailable(GameObject go)
    {
        if (g.assetMgr.ContainsChildByName(go, "KeyHole") == true)
        {
            return true;
        }
        return false;
    }

    bool HasKeyAvailable(GameObject go)
    {
        if (g.assetMgr.ContainsChildByName(go, "Key") == true)
        {
            return true;
        }
        return false;
    }

    void Assemble()
    {
        if (g.ynAssemble == false) return;
//        Debug.Log("Assemble Within\n");
        ConnectAssemblyCurrent();

        g.numCompletedProgress++;
//        g.DebugLog("score " + g.numCompletedProgress);
    }

    void ConnectAssemblyCurrent()
    {
        g.ColorGo(goKeyHoleCurrent, Color.green);
        g.ColorGo(goKeyCurrent, Color.green);
        //
        g.RenameChild(goKeyCurrent, "Key", "KeyLocked");
        g.RenameChild(goKeyHoleCurrent, "KeyHole", "KeyHoleLocked");
        //
        goKeyCurrent.transform.parent = goKeyHoleCurrent.transform;
        //
        goKeyCurrent.transform.localPosition = Vector3.zero;
        goKeyCurrent.transform.localEulerAngles = Vector3.zero;
        //
        g.assetMgr.SetAsset(goKeyHoleCurrent);
        //
        g.assetMgr.AddHelpers();
        //
        g.smoothMgr.LoadLastsAndTargets();
    }

    bool IsNearTargets(GameObject go, GameObject goCheck)
    {
        int n = go.transform.GetSiblingIndex();
        int nCheck = goCheck.transform.GetSiblingIndex();
        float dist = Vector3.Distance(g.smoothMgr.posTargets[n], g.smoothMgr.posTargets[nCheck]);
        if (dist < nearDist)
        {
            return true;
        }
        return false;
    }

    bool IsNear(GameObject go, GameObject goCheck)
    {
        float dist = Vector3.Distance(go.transform.position, goCheck.transform.position);
        if (dist < nearDist)
        {
            return true;
        }
        return false;
    }

    List<GameObject> GetKeyHoles(GameObject goCheck)
    {
        List<GameObject> results = new List<GameObject>();
        Transform[] ts = goCheck.GetComponentsInChildren<Transform>();
        foreach(Transform t in ts)
        {
            GameObject go = t.gameObject;
            if (go.name == "KeyHole")
            {
                results.Add(go.transform.gameObject);
            }
        }
        return results;
    }

    public int GetKeyHoleCount(GameObject go)
    {
        int cnt = 0;
        Transform[] ts = go.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts)
        {
            if (t.name == "KeyHole")
            {
                cnt++;
            }
        }
        return cnt;
    }

    public int GetKeyCount(GameObject go)
    {
        int cnt = 0;
        Transform[] ts = go.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts)
        {
            if (t.name == "Key")
            {
                cnt++;
            }
        }
        return cnt;
    }
}
