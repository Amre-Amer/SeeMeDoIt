using UnityEngine;
using System.Collections.Generic;

public class SmoothMgr : MonoBehaviour
{
    GlobalsMgr g;
    int touchCount;
    int touchCountLast;
    Vector3 scr;
    const float factorRotate = .2f;
    Vector3 posUpDown;
    const float factorUpDown = .001f;
    const float smooth = .2f;
    public List<Vector3> posTargets = new List<Vector3>();
    public List<Vector3> eulTargets = new List<Vector3>();
    public List<Vector3> scaTargets = new List<Vector3>();
    public List<Vector3> posTargetLasts = new List<Vector3>();
    public List<Vector3> eulTargetLasts = new List<Vector3>();
    public List<Vector3> scaTargetLasts = new List<Vector3>();
    const float tolerance = .01f;

    private void Awake()
    {
        g = GetComponent<GlobalsMgr>();
    }

    // Update is called once per frame
    void Update()
    {
        if (g.mode == ModeType.place) return;
        touchCount = g.touchMouseMgr.GetTouchMouseCount();
        UpdatePinch();
        UpdateSmooth();
        touchCountLast = touchCount;
    }

    void UpdatePinch()
    {
        if (g.goAsset == null) return;
        if (touchCountLast != touchCount)
        {
            if (touchCount == 2) // updown
            {
                scr = g.touchMouseMgr.GetTouchMouseScrPosMid();
                posUpDown = GetAssetPosTarget(g.goAsset);
            }
            if (touchCount == 3) //rotate
            {
                scr = g.touchMouseMgr.GetTouchMouseScrPosMidThree();
                g.eulRotate = GetAssetEulTarget(g.goAsset);
            }
        }
        else
        {
            if (touchCount == 2) // updown
            {
                float dy = (g.touchMouseMgr.GetTouchMouseScrPosMid().y - scr.y) * factorUpDown;
                Vector3 posLocal = posUpDown + new Vector3(0, dy, 0);
                UpdateAssetPosTarget(g.goAsset, posLocal);
                GroundAssetPosTarget();
            }
            if (touchCount == 3) // rotate
            {
                float yaw = (g.touchMouseMgr.GetTouchMouseScrPosMidThree().x - scr.x) * factorRotate;
                yaw = -yaw;
                Vector3 eul = g.eulRotate + new Vector3(0, yaw, 0);

                Debug.Log("rot " + eul);
                g.goAsset.transform.localEulerAngles = eul;
                UpdateAssetEulTarget(g.goAsset, eul);
            }
        }
    }

    public Vector3 GetGroundLocalPosFromPos(Vector3 pos)
    {
        return g.goGround.transform.InverseTransformPoint(pos);
    }

    public bool IsPaintOrLine(GameObject go)
    {
        if (IsPaint(go) == true || IsLine(go) == true)
        {
            return true;
        }
        return false;
    }

    public bool IsPaint(GameObject go)
    {
        if (go.name.Contains("Paint_") == true)
        {
            return true;
        }
        return false;
    }

    public bool IsLine(GameObject go)
    {
        if (go.name.Contains("Line_") == true)
        {
            return true;
        }
        return false;
    }

    void UpdateSmooth()
    {
        for (int n = 0; n < g.goAssets.transform.childCount; n++)
        {
            GameObject go = g.goAssets.transform.GetChild(n).gameObject;
            //if (IsPaintOrLine(go) == false)
            //{
                if (WithinTolerance(n) == true)
                {
                    go.transform.localPosition = posTargets[n];
                    go.transform.localEulerAngles = eulTargets[n];
                    go.transform.localScale = scaTargets[n];
                }
                else
                {
                    Vector3 pos = go.transform.localPosition;
                    pos = smooth * posTargets[n] + (1 - smooth) * pos;
                    go.transform.localPosition = pos;
                    //
                    go.transform.localEulerAngles = eulTargets[n];
                    //
                    Vector3 sca = go.transform.localScale;
                    sca = smooth * scaTargets[n] + (1 - smooth) * sca;
                    go.transform.localScale = sca;
                }
            //}
        }
    }

    public void LoadLastsAndTargets()
    {
//        Debug.Log("LoadLastsAndTargets\n");
        posTargets.Clear();
        eulTargets.Clear();
        scaTargets.Clear();
        posTargetLasts.Clear();
        eulTargetLasts.Clear();
        scaTargetLasts.Clear();
        for (int n = 0; n < g.goAssets.transform.childCount; n++)
        {
            GameObject go = g.goAssets.transform.GetChild(n).gameObject;
            if (go.transform.localScale == Vector3.zero)
            {
                Debug.Log("LoadLastsAndTargets " + n + " " + go.name + " zero scale \n");
            }
            posTargets.Add(go.transform.localPosition);
            eulTargets.Add(go.transform.localEulerAngles);
            scaTargets.Add(go.transform.localScale);
            //
            posTargetLasts.Add(go.transform.localPosition);
            eulTargetLasts.Add(go.transform.localEulerAngles);
            scaTargetLasts.Add(go.transform.localScale);
        }
    }

    public void SaveAll()
    {
        for (int n = 0; n < g.goAssets.transform.childCount; n++)
        {
            SaveThisN(n);
        }
    }

    void SaveThisN(int n)
    {
        if (n < 0) return;
        posTargetLasts[n] = posTargets[n];
        eulTargetLasts[n] = eulTargets[n];
        scaTargetLasts[n] = scaTargets[n];
    }

    bool WithinTolerance(int n)
    {
        GameObject go = g.goAssets.transform.GetChild(n).gameObject;
        float distPos = Vector3.Distance(go.transform.localPosition, posTargets[n]);
        float distEul = Vector3.Distance(go.transform.localEulerAngles, eulTargets[n]);
        float distSca = Vector3.Distance(go.transform.localScale, scaTargets[n]);
        bool yn = true;
        if (distPos > tolerance)
        {
            yn = false;
        }
        if (distEul > tolerance)
        {
            yn = false;
        }
        if (distSca > tolerance)
        {
            yn = false;
        }
        return yn;
    }

    public Vector3 GetAssetPosTarget(GameObject go)
    {
        int n = go.transform.GetSiblingIndex();
        if (n < posTargets.Count)
        {
            return posTargets[n];
        }
        return Vector3.zero;
    }

    public Vector3 GetAssetEulTarget(GameObject go)
    {
        int n = go.transform.GetSiblingIndex();
        if (n < eulTargets.Count)
        {
            return eulTargets[n];
        }
        return Vector3.zero;
    }

    public Vector3 GetAssetScaTarget(GameObject go)
    {
        int n = go.transform.GetSiblingIndex();
        if (n < scaTargets.Count)
        {
            return scaTargets[n];
        }
        return Vector3.zero;
    }

    public Vector3 GetAssetPosTargetLast(GameObject go)
    {
        int n = go.transform.GetSiblingIndex();
        if (n < posTargetLasts.Count)
        {
            return posTargetLasts[n];
        }
        return Vector3.zero;
    }

    public Vector3 GetAssetEulTargetLast(GameObject go)
    {
        int n = go.transform.GetSiblingIndex();
        if (n < eulTargetLasts.Count)
        {
            return eulTargetLasts[n];
        }
        return Vector3.zero;
    }

    public Vector3 GetAssetScaTargetLast(GameObject go)
    {
        int n = go.transform.GetSiblingIndex();
        if (n < scaTargetLasts.Count)
        {
            return scaTargetLasts[n];
        }
        return Vector3.zero;
    }

    public void UpdateAssetPos(GameObject go, Vector3 pos)
    {
        int n = go.transform.GetSiblingIndex();
        if (n < posTargets.Count)
        {
            go.transform.localPosition = pos;
        }
    }

    public void UpdateAssetEul(GameObject go, Vector3 eul)
    {
        int n = go.transform.GetSiblingIndex();
        if (n < eulTargets.Count)
        {
            go.transform.localEulerAngles = eul;
        }
    }

    public void UpdateAssetSca(GameObject go, Vector3 sca)
    {
        int n = go.transform.GetSiblingIndex();
        if (n < scaTargets.Count)
        {
            go.transform.localScale = sca;
        }
    }

    public void UpdateAssetPosTargetLast(GameObject go, Vector3 pos)
    {
        int n = go.transform.GetSiblingIndex();
        if (n < posTargetLasts.Count)
        {
            posTargetLasts[n] = pos;
        }
    }

    public void UpdateAssetEulTargeLast(GameObject go, Vector3 eul)
    {
        int n = go.transform.GetSiblingIndex();
        if (n < eulTargetLasts.Count)
        {
            eulTargetLasts[n] = eul;
        }
    }

    public void UpdateAssetScaTargetLast(GameObject go, Vector3 sca)
    {
        int n = go.transform.GetSiblingIndex();
        if (n < scaTargetLasts.Count)
        {
            scaTargetLasts[n] = sca;
        }
    }

    public void UpdateAssetPosTarget(GameObject go, Vector3 pos)
    {
        int n = go.transform.GetSiblingIndex();
        if (n < posTargets.Count)
        {
            posTargets[n] = pos;
        }
    }

    public void UpdateAssetEulTarget(GameObject go, Vector3 eul)
    {
        int n = go.transform.GetSiblingIndex();
        if (n < eulTargets.Count)
        {
            eulTargets[n] = eul;
        }
    }

    public void UpdateAssetScaTarget(GameObject go, Vector3 sca)
    {
        int n = go.transform.GetSiblingIndex();
        if (n < scaTargets.Count)
        {
            scaTargets[n] = sca;
        }
    }

    public void GroundPosTarget(GameObject go)
    {
        Vector3 posLocal = GetAssetPosTarget(go);
        if (posLocal.y < 0)
        {
            posLocal.y = 0;
        }
        UpdateAssetPosTarget(go, posLocal);
    }


    public void GroundAssetPosTarget()
    {
        GroundPosTarget(g.goAsset);
    }

    public void GroundAsset()
    {
        Vector3 posLocal = GetAssetPosTarget(g.goAsset);
        if (posLocal.y < 0)
        {
            posLocal.y = 0;
        }
        UpdateAssetPosTarget(g.goAsset, posLocal);
        g.goAsset.transform.localPosition = posLocal;
    }

}
