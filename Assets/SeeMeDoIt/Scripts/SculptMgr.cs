using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SculptMgr : MonoBehaviour
{
    GlobalsMgr g;
    float distSculpt;
    GameObject goPaint;
    float distNear;
    Vector3 posLast;
    int touchCount;
    int touchCountLast;
    bool ynPaint;
    Vector3 posHit;
    Image imageCenter;
    bool ynSkipLine;
    float scaPaint;
    const float scaPaintOrig = .125f;
    const float shrinkFactor = .5f;
    float scaLine;
    const float ratioLine2Paint = .75f;
    Vector3 posPaint;
    bool ynAutoFull;
    bool ynAutoFullLast;
    float timeStartFull;

    private void Awake()
    {
        g = GetComponent<GlobalsMgr>();
        imageCenter = GameObject.Find("ImageCenter").GetComponent<Image>();
        imageCenter.enabled = false;
        scaPaint = scaPaintOrig;
        scaLine = scaPaint * ratioLine2Paint;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateImageCenter();
        if (g.mode == ModeType.place) return;
        if (g.gameType != GameType.Sculpt) return;
        touchCount = g.touchMouseMgr.GetTouchMouseCount();
        LoadPaint();
        UpdateTouch();
        UpdateSculpt();
        UpdateAuto();
        touchCountLast = touchCount;
    }

    void LoadPaint()
    {
        goPaint = GameObject.Find("Paint");
    }

    void UpdateImageCenter()
    {
        if (g.mode == ModeType.place || g.gameType != GameType.Sculpt)
        {
            imageCenter.enabled = false;
            return;
        }
        imageCenter.enabled = true;
        if (ynPaint == true)
        {
            imageCenter.color = Color.green;
        } else
        {
            imageCenter.color = Color.white;
        }
    }

    public void TurnOffSculpt()
    {
        imageCenter.enabled = false;
    }

    void UpdateAuto()
    {
        if (g.ynAuto == true)
        {
            g.autoMgr.ResetTimeLastAuto();
            AutoSculpt();
        }
    }

    void AutoSculpt()
    {
        ynAutoFull = true;
        if  (CountPaints() < 10)
        {
            ynAutoFull = false;
        }
        if (ynAutoFull == false)
        {
            int numRandom = 10;
            float height = .7f;
            float rad = .5f;
            if (CountPaints() == 0)
            {
                scaPaint = scaPaintOrig;
                scaLine = scaPaint * ratioLine2Paint;
                Vector3 posFrom = g.goGround.transform.position;
                Vector3 posTo = g.goGround.transform.position + Vector3.up * height;
                AddPaint(posTo);
                AddLine(posFrom, posTo);
            }
            else
            {
                for (float r = .1f; r < rad; r += .1f)
                {
                    scaPaint = r / 2;
                    scaLine = scaPaint * .5f;
                    for (int n = 0; n < numRandom; n++)
                    {
                        Vector3 pos = GetPosHtRadRandom(height, 0);
                        AddPaint(pos);
                        Vector3 posTo = GetPosHtRadRandom(height, rad);
                        AddPaint(posTo);
                        AddLine(pos, posTo);
                    }
                }
            }
        }
        if (ynAutoFull != ynAutoFullLast) 
        {
            if (ynAutoFull == true)
            {
                foreach (Transform t in g.goAssets.transform)
                {
                    if (g.smoothMgr.IsPaint(t.gameObject) == true)
                    {
                        Vector3 posLocal = t.localPosition;
                        posLocal.y = 0;
                        g.smoothMgr.UpdateAssetPosTarget(t.gameObject, posLocal);
                    }
                }
                timeStartFull = Time.realtimeSinceStartup;
            }
        }
        if (ynAutoFull == true && Time.realtimeSinceStartup - timeStartFull > 3)
        {
            g.assetMgr.ButtonAdvanceAssetsClickedReLoad();
        }
        ynAutoFullLast = ynAutoFull;
    }

    int CountPaints()
    {
        int cnt = 0;
        foreach(Transform t in g.goAssets.transform)
        {
            if(g.smoothMgr.IsPaintOrLine(t.gameObject) == true)
            {
                cnt++;
            }
        }
        return cnt;
    }

    Vector3 GetPosHtRadRandom(float ht, float rad)
    {
        float x = 0;
        float y = 0;
        float z = 0;
        Vector3 posLocal = new Vector3(x, y, z); 
        Vector3 pos = g.goGround.transform.TransformPoint(Random.insideUnitSphere * rad + Vector3.up * ht);
        return pos;
    }

    Vector3 GetPosPaint()
    {
        Vector3 pos = g.camMain.transform.position + g.camMain.transform.forward * distSculpt;
        return g.smoothMgr.GetGroundLocalPosFromPos(pos);
    }

    void AddMarker(Vector3 pos)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.transform.localScale = Vector3.one * scaPaint;
        go.transform.position = pos;
    }

    void UpdateTouch()
    {
        if (touchCount != touchCountLast)
        {
            if (touchCount > 0)
            {
                ynPaint = !ynPaint;
                if (ynPaint == true)
                {
                    StartPoly();
                }
            } 
        }
    }

    void StartPoly()
    {
        UpdatePosHitDistSculpt();
        posLast = posHit;
        Vector3 pos = GetPosPaint();
        AddPaint(pos);
    }

    void UpdatePosHitDistSculpt()
    {
        ynSkipLine = true;
        RaycastHit hit = new RaycastHit();
        Vector3 scr = new Vector3(Screen.width/2, Screen.height/2, 0);
        Ray ray = Camera.main.ScreenPointToRay(scr);
        Physics.Raycast(ray, out hit, 10);
        if (hit.transform != null)
        {
            ProcessHit(hit);
        }
    }

    void ProcessHit(RaycastHit hit)
    {
        ynSkipLine = false;
        //posHit = hit.point;
        posHit = hit.transform.position;
        distSculpt = Vector3.Distance(posHit, g.camMain.transform.position);
        posHit = g.smoothMgr.GetGroundLocalPosFromPos(posHit);
        scaPaint = scaPaintOrig;
        scaLine = scaPaint * ratioLine2Paint;
        if (hit.transform.name == "PaintBody")
        {
            scaPaint = hit.transform.localScale.x * shrinkFactor;
            scaLine = scaPaint * ratioLine2Paint;
        }
        distNear = scaPaint * 2;
    }

    void UpdateSculpt()
    {
        if (g.ynAuto == true) return;
        if (ynPaint == false) return;
        Vector3 pos = GetPosPaint();
        float dist = Vector3.Distance(posLast, pos);
        if (dist > distNear)
        {
            AddPaint(pos);
            if (ynSkipLine == false)
            {
                AddLine(pos, posLast);
            }
            ynSkipLine = false;
            posLast = pos;
        }
    }

    void AddPaint(Vector3 pos)
    {
        GameObject go = CreatePaint(pos, scaPaint);
        if (g.udpMgr.ynConnected == true) // && g.role == RoleType.sender)
        {
            SendAddPaintRemote(go.name, pos);
        }
        g.autoMgr.ResetTimeLastAuto();
    }

    GameObject CreatePaint(Vector3 pos, float scaXYZ)
    {
        GameObject go = Instantiate(goPaint);
        go.transform.GetChild(0).transform.localScale = Vector3.one * scaXYZ;
        go.transform.parent = g.goAssets.transform;
        go.transform.localPosition = pos;
        g.smoothMgr.LoadLastsAndTargets();
        //
        go.name = "Paint_" + go.transform.parent.childCount + "_" + Time.realtimeSinceStartup;
        return go;
    }

    void AddLine(Vector3 posFrom, Vector3 posTo)
    {
        GameObject go = CreateLine(posFrom, posTo, scaLine);
        if (g.udpMgr.ynConnected == true) // && g.role == RoleType.sender)
        {
            SendAddLineRemote(go.name, posFrom, posTo, scaLine);
        }
        g.autoMgr.ResetTimeLastAuto();
    }

    GameObject CreateLine(Vector3 posFrom, Vector3 posTo, float scaZ) 
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.transform.parent = g.goAssets.transform;
        RemoveCapsuleCollider(go);
        go.transform.localPosition = (posFrom + posTo) / 2;
        go.transform.LookAt(g.goGround.transform.TransformPoint(posTo));
        float dist = Vector3.Distance(posFrom, posTo);
        go.transform.Rotate(90, 0, 0);
        go.transform.localScale = new Vector3(scaZ, dist / 2, scaZ);
        g.ColorGo(go, ColorLine(scaZ));
        g.smoothMgr.LoadLastsAndTargets();
        //
        go.name = "Line_" + go.transform.parent.childCount + "_" + Time.realtimeSinceStartup;
        return go;
    }

    string SerializePaint(string txtName, Vector3 pos, float scaXYZ)
    {
        string txt = txtName; 
        txt += "/" + pos.x.ToString(g.f8) + "/" + pos.y.ToString(g.f8) + "/" + pos.z.ToString(g.f8);
        txt += "/" + scaXYZ.ToString(g.f8);
        return txt;
    }

    void UnSerializePaint(string txt)
    {
        if (txt + "" == "") return;
        Vector3 pos = Vector3.zero;
        string[] stuff = txt.Split('/');
        if (stuff.Length != 5) return;
        string txtName = stuff[0];
        pos.x = g.udpMgr.FloatParse(stuff[1]);
        pos.y = g.udpMgr.FloatParse(stuff[2]);
        pos.z = g.udpMgr.FloatParse(stuff[3]);
        float scaXYZ = g.udpMgr.FloatParse(stuff[4]);
        //
        if (DoesAssetExistByName(txtName) == false)
        {
            GameObject go = CreatePaint(pos, scaXYZ);
            go.name = txtName;
            //AddReceivedPaintLine(txt);
        }
    }

    bool DoesAssetExistByName(string txt)
    {
        if (g.assetMgr.GetChildByName(g.goAssets, txt) == true)
        {
            return true;
        }
        return false;
    }

    string SerializeLine(string txtName, Vector3 posFrom, Vector3 posTo, float scaZ)
    {
        string txt = txtName; 
        txt += "/" + posFrom.x.ToString(g.f8) + "/" + posFrom.y.ToString(g.f8) + "/" + posFrom.z.ToString(g.f8);
        txt += "/" + posTo.x.ToString(g.f8) + "/" + posTo.y.ToString(g.f8) + "/" + posTo.z.ToString(g.f8);
        txt += "/" + scaZ.ToString(g.f8);
        return txt;
    }

    void UnSerializeLine(string txt)
    {
        if (txt + "" == "") return;
        Vector3 posFrom = Vector3.zero;
        Vector3 posTo = Vector3.zero;
        float scaZ = 1;
        string[] stuff = txt.Split('/');
        if (stuff.Length != 8) return;
        string txtName = stuff[0];
        posFrom.x = g.udpMgr.FloatParse(stuff[1]);
        posFrom.y = g.udpMgr.FloatParse(stuff[2]);
        posFrom.z = g.udpMgr.FloatParse(stuff[3]);
        posTo.x = g.udpMgr.FloatParse(stuff[4]);
        posTo.y = g.udpMgr.FloatParse(stuff[5]);
        posTo.z = g.udpMgr.FloatParse(stuff[6]);
        scaZ = g.udpMgr.FloatParse(stuff[7]);
        //
        if (DoesAssetExistByName(txtName) == false)
        {
            GameObject go = CreateLine(posFrom, posTo, scaZ);
            go.name = txtName;
        }
    }

    void RemoveCapsuleCollider(GameObject go)
    {
        CapsuleCollider cc = go.GetComponent<CapsuleCollider>();
        if (cc != null)
        {
            Destroy(cc);
        }
    }

    void SendAddPaintRemote(string txtName, Vector3 pos)
    {
        string txtKey = "AddPaint";
        string txtValue = SerializePaint(txtName, pos, scaPaint);
        string txt = g.udpMgr.CreateSendKeyValue(txtKey, txtValue);
        g.udpMgr.AddCreateSendKeyValueToTxtConfirmsExpires(txt); // amre
    }

    void SendAddLineRemote(string txtName, Vector3 posFrom, Vector3 posTo, float scaZ)
    {
        string txtKey = "AddLine";
        string txtValue = SerializeLine(txtName, posFrom, posTo, scaZ);
        string txt  = g.udpMgr.CreateSendKeyValue(txtKey, txtValue);
        g.udpMgr.AddCreateSendKeyValueToTxtConfirmsExpires(txt); // amre
    }

    public void AddPaintRemote(string txt)
    {
        if (g.gameType != GameType.Sculpt) return;
        UnSerializePaint(txt);
    }

    public void AddLineRemote(string txt)
    {
        if (g.gameType != GameType.Sculpt) return;
        UnSerializeLine(txt);
    }

    Color ColorLine(float sca)
    {
        float c = sca / scaPaintOrig;
        return new Color(c, c, c);
    }

    public int GetNumTotalProgress()
    {
        return 0;
    }
}
