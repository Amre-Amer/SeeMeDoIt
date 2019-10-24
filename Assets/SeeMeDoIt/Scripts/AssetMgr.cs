using UnityEngine;
using System.Collections.Generic;
using System;

public class AssetMgr : MonoBehaviour
{
    GlobalsMgr g;
    const float delayShrink = .5f;
    const float delayExpand = .5f;
    const float pauseBeforeAdvance = 1;

    private void Awake()
    {
        g = GetComponent<GlobalsMgr>();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject go = g.goAssetsHolder.transform.GetChild(0).gameObject;
        SetAssets(go);
        g.goAssetsHolder.SetActive(false);
        g.goSenderCamOrig.SetActive(false);
        g.goReceiverCamOrig.SetActive(false);
        g.goPointerOrig.SetActive(false);
    }

    public List<GameObject> GetListPieces()
    {
        List<GameObject> pieces = new List<GameObject>();
        foreach (Transform t in g.goAssets.transform)
        {
            if (IsPiece(t.gameObject) == true)
            {
                pieces.Add(t.gameObject);
            }
        }
        return pieces;
    }

    public void MoveAssetToAsset(GameObject goPiece, GameObject go)
    {
        g.smoothMgr.UpdateAssetPosTarget(goPiece, go.transform.localPosition);
        //int n = goPiece.transform.GetSiblingIndex();
        //Debug.Log(g.ynAuto + " " + g.udpMgr.ynConnected + " " + g.ynPause + " " + goPiece.name + " moved " + g.udpMgr.HasTargetMoved(n) + "\n");
    }

    public bool IsPiece(GameObject go)
    {
        if (go.name != "SenderCam" && go.name != "ReceiverCam" && go.name != "Pointer")
        {
            return true;
        }
        return false;
    }

    void ClearImage()
    {
        g.imagePlane.gameObject.SetActive(false);
    }

    void MatchPosEul(GameObject goCopy, GameObject goOrig)
    {
        goCopy.transform.position = goOrig.transform.position;
        goCopy.transform.eulerAngles = goOrig.transform.eulerAngles;
    }

    void AdvanceAssetsHolder(bool ynReload)
    {
        if (ynReload == false)
        {
            g.nAssetsHolder++;
        }
        if (g.nAssetsHolder == g.goAssetsHolder.transform.childCount)
        {
            g.nAssetsHolder = 0;
        }
    }

    public void ButtonAdvanceAssetsClickedReLoad()
    {
        g.pointerMgr.FlashAdvance();
        g.ynPause = true;
        AdvanceAssets(true);
    }

    public void ButtonAdvanceAssetsClicked() 
    {
        if (g.ynPause == true) return;
        g.pointerMgr.FlashAdvance();
        g.ynPause = true;
        AdvanceAssets(false);
    }

    public void ButtonRetreatAssetsClicked()
    {
        g.pointerMgr.FlashRetreat();
        g.ynPause = true;
        RetreatAssets();
    }

    public void ButtonCloudAssetsClicked()
    {
        g.pointerMgr.FlashCloud();
        g.ynPause = true;
        DownloadNextCloudAssets();
    }

    void DownloadNextCloudAssets()
    {
        Debug.Log("DownloadNextCloudAssets\n");
        g.assetBundlesMgr.StartDownload();
    }

    public void RetreatAssets()
    {
        RetreatAssetsHolder();
        AdvanceShrinkAutoAdvance();
    }

    void RetreatAssetsHolder()
    {
        g.nAssetsHolder--;
        if (g.nAssetsHolder < 0)
        {
            g.nAssetsHolder = g.goAssetsHolder.transform.childCount - 1;
        }
    }

    public void AdvanceAssets(bool ynReload)
    {
        AdvanceAssetsHolder(ynReload);
        AdvanceShrinkAutoAdvance();
    }

    public void AdvanceShrinkAutoAdvance()
    {
        AdvanceShrink();
        Invoke("Advance", delayShrink);
    }

    public void AdvanceShrink()
    {
        SetAssetsChildrenScale(1);
        SetAssetsChildrenScaleTargets(0);
    }

    public void Advance()  
    {
        g.DebugLog("Advance...");
        //g.shakeMgr.AdvanceTableMaterial();
        g.ticTacToeMgr.ColorSideIndicators((Color.white + Color.black) / 2);
        //
        SwitchAssets();
        //
        AdvanceExpand();
        g.ticTacToeMgr.ResetTicTacToe();
        Invoke("ClearPause", delayExpand);
    }

    public void SwitchToExternalAssets(GameObject go)
    {
        g.goAssetsHolder.SetActive(true);
        go.transform.parent = g.goAssetsHolder.transform;
        go.transform.localPosition = Vector3.zero;
        g.nAssetsHolder = go.transform.GetSiblingIndex();
        g.goAssetsHolder.SetActive(false);
        SwitchAssets();
    }

    public void SwitchToAssetsByName(string txt)
    {
        GameObject go = GetChildByName(g.goAssetsHolder, txt);
        if (go != null)
        {
            SwitchToExternalAssets(go);
        } else
        {
            Debug.Log("!SwitchToAssetsByName " + txt + "\n");
        }
    }

    void SwitchAssets()
    {
        g.goAssetsHolder.SetActive(true);
        GameObject goAssetsNew = g.goAssetsHolder.transform.GetChild(g.nAssetsHolder).gameObject;
        SetAssets(goAssetsNew);
        g.goAssetsHolder.SetActive(false);

        if (g.udpMgr.ynConnected == true && g.ynPause == true)
        {
            SendAdvanceRemote(goAssetsNew.name);
        }
    }

    public void SendAdvanceRemote(string txtNewAssetsName)
    {
        string txtKey = "Advance";
        string txtValue = txtNewAssetsName + "@" + Time.realtimeSinceStartup;
        string txt = g.udpMgr.CreateSendKeyValue(txtKey, txtValue);
        g.udpMgr.AddCreateSendKeyValueToTxtConfirmsExpires(txt);
        Debug.Log("SendAdvanceRemote " + txtValue + "\n");
    }

    public void AddAdvanceRemote(string txt)
    {
        UnSerializeAdvance(txt);
    }

    void UnSerializeAdvance(string txt)
    {
        if (g.udpMgr.ExistsInReceives(txt) == true) return;
        g.udpMgr.AddToReceives(txt);
        string[] stuff = txt.Split('@');
        SwitchToAssetsByName(stuff[0]);
        Debug.Log("AdvanceRemote " + stuff[0] + "\n");
    }

    public void AdvanceExpand()
    {
        SetAssetsChildrenScale(0);
        SetAssetsChildrenScaleTargets(1);
    }

    void ClearPause()
    {
        g.ynPause = false;
    }

    public void SetAssets(GameObject goAssetsNew)
    {
        if (g.goAssets != null)
        {
            Destroy(g.goAssets);
        }
//        Handheld.Vibrate();
        g.goAssets = Instantiate(goAssetsNew, g.goGround.transform);
        g.goAssets.name = "Assets (" + goAssetsNew.name + ")";
        g.goAssets.SetActive(true);
        SetAsset(GetFirstAsset());
        AddHelpers();
        g.smoothMgr.LoadLastsAndTargets();
        UpdateGameType();
        g.UpdateFps();
        g.progressMgr.CreateProgress();
    }

    public void AddHelpers()
    {
        RemoveChildByName(g.goAssets, "ReceiverCam");
        RemoveChildByName(g.goAssets, "SenderCam");
        RemoveChildByName(g.goAssets, "Pointer");
        //
        Vector3 posLocal = Vector3.zero;
        Vector3 eulLocal = Vector3.zero;
        if (g.goReceiverCam != null)
        {
            posLocal = g.goReceiverCam.transform.localPosition;
            eulLocal = g.goReceiverCam.transform.localEulerAngles;
        }
        g.goReceiverCam = Instantiate(g.goReceiverCamOrig, g.goAssets.transform);
        g.goReceiverCam.transform.localPosition = posLocal;
        g.goReceiverCam.transform.localEulerAngles = eulLocal;
        g.goReceiverCam.SetActive(g.udpMgr.ynConnected);
        g.goReceiverCam.name = "ReceiverCam";
        //
        posLocal = Vector3.zero;
        if (g.goSenderCam != null)
        {
            posLocal = g.goSenderCam.transform.localPosition;
            eulLocal = g.goSenderCam.transform.localEulerAngles;
        }
        g.goSenderCam = Instantiate(g.goSenderCamOrig, g.goAssets.transform);
        g.goSenderCam.transform.localPosition = posLocal;
        g.goSenderCam.transform.localEulerAngles = eulLocal;
        g.goSenderCam.SetActive(g.udpMgr.ynConnected);
        g.goSenderCam.name = "SenderCam";
        //
        //RestoreCams();
        //
        g.goPointerOrig.SetActive(true);
        g.goPointer = Instantiate(g.goPointerOrig, g.goAssets.transform);
        //
        g.goPointer.transform.localScale = Vector3.one;
        g.goPointerOrig.SetActive(false);
        g.goPointer.name = "Pointer";
    }

    void UpdateGameType()
    {
        //if (g.goAssets.name.Contains(GameType.Connect.ToString()) == true)
        //{
        //    g.gameType = GameType.Connect;
        //}
        //if (g.goAssets.name.Contains(GameType.TicTacToe.ToString()) == true)
        //{
        //    g.gameType = GameType.TicTacToe;
        //}
        //if (g.goAssets.name.Contains(GameType.Screw.ToString()) == true)
        //{
        //    g.gameType = GameType.Screw;
        //}
        //if (g.goAssets.name.Contains(GameType.Chess.ToString()) == true)
        //{
        //    g.gameType = GameType.Chess;
        //}
        //if (g.goAssets.name.Contains(GameType.Pong.ToString()) == true)
        //{
        //    g.gameType = GameType.Pong;
        //}
        //if (g.goAssets.name.Contains(GameType.Sculpt.ToString()) == true)
        //{
        //    g.gameType = GameType.Sculpt;
        //}
        //if (g.goAssets.name.Contains(GameType.Noise.ToString()) == true)
        //{
        //    g.gameType = GameType.Noise;
        //}
        //if (g.goAssets.name.Contains(GameType.Towers.ToString()) == true)
        //{
        //    g.gameType = GameType.Towers;
        //}
        //SetGameTypeByString(GameType.Art);
        //if (g.goAssets.name.Contains(GameType.Art.ToString()) == true)
        //{
        //    g.gameType = GameType.Art;
        //}
        foreach (GameType gameType in Enum.GetValues(typeof(GameType)))
        {
            SetGameTypeByString(gameType);
        }
        //if (g.goAssets.name.Contains(GameType.Learn.ToString()) == true)
        //{
        //    g.gameType = GameType.Learn;
        //}
        //SetGameTypeByString(GameType.Learn);
        g.DebugLog("GameType " + g.gameType);
    }

    void SetGameTypeByString(GameType gameType)
    {
        if (g.goAssets.name.Contains(gameType.ToString()) == true)
        {
            g.gameType = gameType;
        }
    }

    public void SetAsset(GameObject go)
    {
        g.goAsset = go;
        //
        g.smoothMgr.UpdateAssetPosTarget(g.goAsset, g.goAsset.transform.localPosition);
        g.smoothMgr.UpdateAssetEulTarget(g.goAsset, g.goAsset.transform.localEulerAngles);
        //
        g.smoothMgr.UpdateAssetPosTargetLast(g.goAsset, g.goAsset.transform.localPosition);
        g.smoothMgr.UpdateAssetEulTargeLast(g.goAsset, g.goAsset.transform.localEulerAngles);
        //
        g.boundsMgr.UpdateBoundingBox();
        //
        int n = g.goAsset.transform.GetSiblingIndex();
    }

    void RemoveChildByName(GameObject goParent, string txt)
    {
        if (goParent.transform.Find(txt) != null)
        {
            GameObject go = goParent.transform.Find(txt).gameObject;
            if (go != null)
            {
                Destroy(go);
            }
        }
    }

    public GameObject GetFirstAssets()
    {
        return g.goAssetsHolder.transform.GetChild(0).gameObject;
    }

    public GameObject GetFirstAsset()
    {
        return g.goAssets.transform.GetChild(0).gameObject; 
    }

    public void SetAssetsChildrenScaleTargets(float sca)
    {
        for (int n = 0; n < g.goAssets.transform.childCount; n++)
        {
            g.smoothMgr.scaTargets[n] = Vector3.one * sca;
        }
    }

    public void SetAssetsChildrenScale(float sca)
    {
        for(int n = 0; n < g.goAssets.transform.childCount; n++)
        {
            GameObject go = g.goAssets.transform.GetChild(n).gameObject;
            go.transform.localScale = Vector3.one * sca;
            g.smoothMgr.scaTargets[n] = Vector3.one * sca;
            g.smoothMgr.scaTargetLasts[n] = Vector3.one * sca;
        }
    }

    void SetMaterials(GameObject go, Material mat)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
        foreach(Renderer rend in renderers)
        {
            rend.material = mat;
        }
    }

    public GameObject FindAssetParent(GameObject go)
    {
        foreach (Transform t in g.goAssets.transform)
        {
            if (ContainsChild(t.gameObject, go) == true)
            {
                return t.gameObject;
            }
        }
        return null;
    }

    public bool ContainsChild(GameObject goParent, GameObject goChild)
    {
        if (goParent == goChild)
        {
            return true;
        }
        Transform[] ts = goParent.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts)
        {
            if (t.gameObject == goChild)
            {
                return true;
            }
        }
        return false;
    }

    public GameObject GetChildByName(GameObject go, string txt)
    {
        if (go.name == txt)
        {
            return go;
        }
        Transform[] ts = go.GetComponentsInChildren<Transform>();
        foreach(Transform t in ts)
        {
            if (t.name == txt)
            {
                return t.gameObject;
            }
        }
        return null;
    }

    public bool ContainsChildByName(GameObject goParent, string txt)
    {
        if (goParent.name == txt)
        {
            return true;
        }
        Transform[] ts = goParent.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts)
        {
            if (t.name == txt)
            {
                return true;
            }
        }
        return false;
    }
}
