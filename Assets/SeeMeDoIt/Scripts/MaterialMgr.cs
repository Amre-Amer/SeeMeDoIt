using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialMgr : MonoBehaviour
{
    GlobalsMgr g;
    public Material mat;
    public Texture2D[] texs;
    int nTex = -1;

    private void Awake()
    {
        g = GetComponent<GlobalsMgr>();
    }

    public void AdvanceTableMaterial()
    {
        nTex++;
        if (nTex == texs.Length)
        {
            nTex = 0;
        }
        mat.mainTexture = texs[nTex];
        string txt = texs[nTex].name;
        float scaleX = 1;
        float scaleY = 1;
        switch (txt)
        {
            case "art":
                scaleX = 1;
                scaleY = 1;
                break;
            case "bamboo":
                scaleX = 2;
                scaleY = 2;
                break;
            case "bubbles":
                scaleX = 1;
                scaleY = 1;
                break;
            case "cardboard":
                scaleX = 2;
                scaleY = 2;
                break;
            case "checker":
                scaleX = 1;
                scaleY = 1;
                break;
            case "grid":
                scaleX = 2;
                scaleY = 2;
                break;
            case "marble":
                scaleX = 1;
                scaleY = 1;
                break;
            case "rubber":
                scaleX = 2;
                scaleY = 2;
                break;
            case "wood":
                scaleX = 1;
                scaleY = 1;
                break;
            default:
                break;
        }
        mat.mainTextureScale = new Vector2(scaleX, scaleY);
    }
}
