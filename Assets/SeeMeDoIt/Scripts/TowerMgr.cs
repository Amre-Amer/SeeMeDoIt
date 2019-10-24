using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerMgr : MonoBehaviour
{
    GlobalsMgr g;
    const float distNear = .1f;
    List<GameObject> towers = new List<GameObject>();
    //float timeStart;
    //const float delay = 1;

    private void Awake()
    {
        g = GameObject.Find("SeeMeDoIt").GetComponent<GlobalsMgr>();
    }

    // Update is called once per frame
    void Update()
    {
        if (g.mode == ModeType.place) return;
        if (g.gameType != GameType.Towers) return;
        LoadTowers();
        UpdateVibrate();
        //UpdateTowers();
        UpdateAuto();
    }

    void UpdateVibrate()
    {
        int n = Random.Range(0, towers.Count);
        GameObject go = towers[n];
        Vector3 posLocal = g.smoothMgr.GetAssetPosTarget(go);
        Vector3 posRandom = Random.insideUnitCircle * .1f;
        posRandom.z = posRandom.y;
        posRandom.y = 0;
        posLocal += posRandom;
        g.smoothMgr.UpdateAssetPosTarget(go, posLocal);
    }

    void UpdateAuto()
    {
        if (g.ynAuto == true)
        {
            g.autoMgr.ResetTimeLastAuto();
            AutoTowers();
        }
    }

    void AutoTowers()
    {

    }

    void UpdateTowers()
    {
        ColorTowers(Color.white);
        foreach (GameObject go in towers)
        {
            GameObject goNear = FindNearTower(go);
            if (goNear != null)
            {
                Debug.Log("UpdateTowers\n");
                ColorTowers(Color.green);
                return;
            }
        }
    }

    void LoadTowers()
    {
        towers.Clear();
        foreach (Transform t in g.goAssets.transform)
        {
            if (IsTower(t.gameObject) == true)
            {
                towers.Add(t.gameObject);
            }
        }
    }

    void ColorTowers(Color color)
    {
        foreach (GameObject go in towers)
        {
            g.ColorGo(go, color);
        }
    }

    GameObject FindNearTower(GameObject goCheck)
    {
        foreach (GameObject go in towers)
        {
            if (go != goCheck)
            {
                if (g.IsNear(go, goCheck, distNear) == true)
                {
                    return go;
                }
            }
        }
        return null;
    }

    bool IsTower(GameObject go)
    {
        if (go.name.Contains("Tower") == true)
        {
            return true;
        }
        return false;
    }
}
