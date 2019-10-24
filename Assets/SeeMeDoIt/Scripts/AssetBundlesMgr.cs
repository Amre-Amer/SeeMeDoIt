using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;
using System.Collections.Generic;

public class AssetBundlesMgr : MonoBehaviour
{
    GlobalsMgr g;
    GameObject goAssetBundle;
    UnityWebRequest uwr;
    //string urlX ="https://drive.google.com/file/d/1ytxjXlsm6cqD4K4Q0buxzJUNOwgRT7IQ/view?usp=sharing";
    string url = "https://drive.google.com/uc?export=view&id=1ytxjXlsm6cqD4K4Q0buxzJUNOwgRT7IQ";
    string assetNameCurrent = "";

    private void Awake()
    {
        g = GetComponent<GlobalsMgr>();
    }

    // Start is called before the first frame update
    void Start()
    {
        g.progressMgr.HideImageProgress();
        //Invoke("StartDownload", 3);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void StartDownload()
    {
        if (goAssetBundle != null)
        {
            Destroy(goAssetBundle);
        }
        StartCoroutine(GetAssetBundle());
    }

    IEnumerator GetAssetBundle()
    {
        Caching.ClearCache();
        g.progressMgr.UpdateImageProgress(-1);
        using (uwr = UnityWebRequestAssetBundle.GetAssetBundle(url))
        {
            AsyncOperation request = uwr.SendWebRequest();
            while (!request.isDone)
            {
                g.progressMgr.UpdateImageProgress(uwr.downloadProgress);
                yield return null;
            }
            g.progressMgr.UpdateImageProgress(2);
            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(uwr);
                UpdateNextAssetNameCurrent(assetBundle);
                goAssetBundle = Instantiate(assetBundle.LoadAsset(assetNameCurrent)) as GameObject;
                g.assetMgr.SwitchToExternalAssets(goAssetBundle);
                assetBundle.Unload(false);
                uwr = null;
            }
        }
    }

    void UpdateNextAssetNameCurrent(AssetBundle assetBundle)
    {
        string[] stuff = assetBundle.GetAllAssetNames();
        if (stuff.Length == 0)
        {
            assetNameCurrent = "";
            return;
        }
        List<string> stuffList = GetPrefabListFromStringArray(stuff);
        int n = stuffList.IndexOf(assetNameCurrent);
        n++;
        if (n >= stuffList.Count)
        {
            n = 0;
        }
        assetNameCurrent = stuffList[n];
    }

    List<string>GetPrefabListFromStringArray(string[] stuff)
    {
        Debug.Log("assetList..............\n");
        List<string> stuffList = new List<string>();
        for(int n = 0; n < stuff.Length; n++)
        {
            string txt = stuff[n];
            if (txt.Contains(".prefab") == true)
            {
                stuffList.Add(txt);
//                Debug.Log("asset " + txt + "\n");
            }
        }
        return stuffList;
    }
}
