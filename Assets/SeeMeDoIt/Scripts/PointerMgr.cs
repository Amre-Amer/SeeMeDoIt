using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerMgr : MonoBehaviour
{
    GlobalsMgr g;
    int touchCount;
    bool ynNear;
    const float distNear = .06f;
    const float delay = .5f;
    const float delayBussy = .5f;
    bool ynBusy;

    private void Awake()
    {
        g = GetComponent<GlobalsMgr>();
    }

    // Start is called before the first frame update
    void Start()
    {
        ClearAdvanceLight();
    }

    // Update is called once per frame
    void Update()
    {
        if (g.mode == ModeType.place) return;
        touchCount = g.touchMouseMgr.GetTouchMouseCount();
        UpdatePointer();
    }

    GameObject FindNearestAdvanceToPointerX()
    {
        float distMin = 1000;
        GameObject goMin = null;
        foreach(GameObject go in g.goAdvances)
        {
            float dist = Vector3.Distance(go.transform.position, g.goPointer.transform.position);
            if (dist < distMin)
            {
                distMin = dist;
                goMin = go;
            }
        }
        return goMin;
    }

    void UpdatePointer()
    {
        if (touchCount == 0) return;
        RaycastHit hit = new RaycastHit();
        Vector3 scr = g.touchMouseMgr.GetTouchMouseScrPos();
        Ray ray = Camera.main.ScreenPointToRay(scr);
        Physics.Raycast(ray, out hit, 10, g.layerContent | g.layerGround | g.layerAdvance); 
        if (hit.transform != null)
        {
            Vector3 pos = g.goGround.transform.InverseTransformPoint(hit.point); // amre
            g.goPointer.transform.localPosition = pos;
            g.smoothMgr.UpdateAssetPosTarget(g.goPointer, pos);
        }
    }

    public void FlashAdvance()
    {
        foreach (GameObject go in g.goAdvanceLights)
        {
            Material mat = go.GetComponent<Renderer>().material;
            mat.SetColor("_EmissionColor", Color.green / 2);
        }
        Invoke("ClearAdvanceLight", delay);
    }

    public void FlashRetreat()
    {
        foreach (GameObject go in g.goRetreatLights)
        {
            Material mat = go.GetComponent<Renderer>().material;
            mat.SetColor("_EmissionColor", Color.green / 2);
        }
        Invoke("ClearRetreatLight", delay);
    }

    public void FlashCloud()
    {
        foreach (GameObject go in g.goCloudLights)
        {
            Material mat = go.GetComponent<Renderer>().material;
            mat.SetColor("_EmissionColor", Color.green / 2);
        }
        Invoke("ClearCloudLight", delay);
    }

    void ClearCloudLight()
    {
        foreach (GameObject go in g.goCloudLights)
        {
            Material mat = go.GetComponent<Renderer>().material;
            mat.SetColor("_EmissionColor", Color.black);
        }
    }
    void ClearAdvanceLight()
    {
        foreach (GameObject go in g.goAdvanceLights)
        {
            Material mat = go.GetComponent<Renderer>().material;
            mat.SetColor("_EmissionColor", Color.black);
        }
    }

    void ClearRetreatLight()
    {
        foreach (GameObject go in g.goRetreatLights)
        {
            Material mat = go.GetComponent<Renderer>().material;
            mat.SetColor("_EmissionColor", Color.black);
        }
    }
}
