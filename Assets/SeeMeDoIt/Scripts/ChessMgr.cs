using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessMgr : MonoBehaviour
{
    GlobalsMgr g;

    private void Awake()
    {
        g = GetComponent<GlobalsMgr>();    
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (g.mode == ModeType.place) return;
        if (g.gameType != GameType.Chess) return;
        UpdateAuto();
    }

    public int GetNumTotalProgress()
    {
        return 0;
    }

    void UpdateAuto()
    {
        if (g.ynAuto == true)
        {
            g.autoMgr.ResetTimeLastAuto();
            Rearrange();
        }
    }

    void Rearrange()
    {
        List<GameObject> goPiecesList = g.assetMgr.GetListPieces();
        foreach(GameObject go in goPiecesList)
        {
            GameObject goOther = GetOtherColorPiece(go);
            g.smoothMgr.UpdateAssetPosTarget(go, goOther.transform.localPosition);
            g.smoothMgr.UpdateAssetPosTarget(goOther, go.transform.localPosition);
        }
    }

    GameObject GetOtherColorPiece(GameObject go)
    {
        string txt = "";
        if (go.name.Contains("White") == true)
        {
            txt = go.name.Replace("White", "Black");
        } else
        {
            txt = go.name.Replace("Black", "White");
        }
        return g.assetMgr.GetChildByName(g.goAssets, txt);
    }
}
