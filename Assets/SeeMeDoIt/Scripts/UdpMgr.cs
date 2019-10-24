using UnityEngine;
using System.Net.Sockets;
using System.Threading;
using System;
using System.Net;
using UnityEngine.UI;
using System.Collections.Generic;

public class UdpMgr : MonoBehaviour
{
    GlobalsMgr g;
    UdpClient client;
    Thread receiveThread;
    public int port;
    IPEndPoint remoteEndPointSender;
    public Vector3 posReceived;
    public Vector3 eulReceived;
    public Vector3 scaReceived;
    public string nameReceived;
    public string keyReceived;
    public string valueReceived;
    public string txtReceived;
    bool ynReceived;
    int cntFrameSends;
    //
    int touchCountLast;
    int touchCount;
    float ang;
    byte[] data;
    public int secondsSince1970;
    public int secondsSince1970other;
    public float timeLastReceived;
    public float timeLastReceivedTimeout;
    public int bytesSent;
    public int bytesReceived;
    public int maxPayload;
    public string ipLocal;
    public string ipFrom;
    public string txtError;
    public bool ynError;
    public Image imageConnected;
    public bool ynConnected;
    const float tolerance = .001f;
    const int maxDataSegmentLength = 1000; 
    int nDataSegment;
    int nDataSegmentNum;
    string txtConfirms = "";
    string txtSend = "";
    const int expireCycleCountLimit = 5; //10;
    List<string> expires = new List<string>();
    List<int> expireCycleCounts = new List<int>();
    List<string> receives = new List<string>();
    List<float> receivesTime = new List<float>();
    const float receivesTimeLimit = 1f;
    public bool ynForceYnConnected;

    private void Awake()
    {
        g = GetComponent<GlobalsMgr>();
        imageConnected = GameObject.Find("ImageConnected").GetComponent<Image>();
        timeLastReceivedTimeout = 3;
        secondsSince1970 = GetSecondsSince1970();
        ipLocal = "?";
        ipFrom = "?";
        ynForceYnConnected = false;  // false;
    }

    // Start is called before the first frame update
    void Start()
    {
        ipLocal = GetIpLocal();
        DebugLog("ip local " + ipLocal);
        StartUdp();
        InvokeRepeating("SendStartTimeApp", 1, 1);
        InvokeRepeating("UpdateRole", timeLastReceivedTimeout, timeLastReceivedTimeout);
        DebugLog("Local Ip Address " + GetLocalIPAddress());
        SetConnected(false);
    }

    void Test()
    {
        if (ynConnected == true && g.role == RoleType.sender)
        {
            string txt = g.udpMgr.CreateSendKeyValue("Flash", "Yes");
            g.udpMgr.AddCreateSendKeyValueToTxtConfirmsExpires(txt); // amre
        }
    }

    // Update is called once per frame
    void Update()
    {
        touchCount = g.touchMouseMgr.GetTouchMouseCount();
        UpdateChangeN();
        UpdateReceives();
        touchCountLast = touchCount;
    }

    public void AddToReceives(string txt)
    {
        receives.Add(txt);
        receivesTime.Add(Time.realtimeSinceStartup);
    }

    public bool ExistsInReceives(string txt)
    {
        if (receives.Contains(txt) == true)
        {
            return true;
        }
        return false;
    }

    void UpdateReceives()
    {
        for(int n = 0; n < receives.Count; n++)
        {
            if (Time.realtimeSinceStartup - receivesTime[n] > receivesTimeLimit)
            {
                RemoveFromReceivesAtIndex(n);
            }
        }
    }

    void RemoveFromReceivesAtIndex(int n)
    {
        receives.RemoveAt(n);
        receivesTime.RemoveAt(n);
    }

    void UpdateRole()
    {
        if (Time.realtimeSinceStartup - timeLastReceived > timeLastReceivedTimeout)
        {
            SetConnected(false);
            g.role = RoleType.sender;
            ipFrom = "?";
//            DebugLog("====  time out ==== role " + g.role);
        }
    }

    void SetConnected(bool yn)
    {
        if (ynForceYnConnected == true)
        {
            yn = true; // test
        }
        imageConnected.gameObject.SetActive(yn);
        ynConnected = yn;
    }

    public bool HasTargetMoved(int n)
    {
        float distPos = Vector3.Distance(g.smoothMgr.posTargets[n], g.smoothMgr.posTargetLasts[n]);
        float distEul = Vector3.Distance(g.smoothMgr.eulTargets[n], g.smoothMgr.eulTargetLasts[n]);
        float distSca = Vector3.Distance(g.smoothMgr.scaTargets[n], g.smoothMgr.scaTargetLasts[n]);
        if (distPos > tolerance || distEul > tolerance || distSca > tolerance)
        {
            return true;
        }
        return false;
    }

    void UpdateChangeN()
    {
        GameObject goCam = g.goSenderCam;
        if (g.role == RoleType.receiver)
        {
            goCam = g.goReceiverCam;
        }
        txtSend = "";
        for (int n = 0; n < g.goAssets.transform.childCount; n++)
        {
            GameObject go = g.goAssets.transform.GetChild(n).gameObject;
            if (g.ynPause == false)
            {
                if (g.udpMgr.ynConnected == true && HasTargetMoved(n) == true)
                {
                    if (txtSend != "")
                    {
                        txtSend += "*";
                    }
                    txtSend += Vector3ToStringGoNtarget(n);
                }
            }
        }
        UpdateExpiredFromTxtConfirms();
        AddConfirms();
        if (txtSend != "")
        {
            SendString(txtSend);
        }

        if (ynReceived == true)
        {
            if (ynError == true)
            {
                DebugLog(txtError);
                ynError = false;
            }
            else
            {
                g.DebugLog("Receiving " + txtReceived);
                ReceiveOtherNs();
                timeLastReceived = Time.realtimeSinceStartup;
                ynReceived = false;
                txtReceived = "";
            }
        }
        g.smoothMgr.SaveAll();
    }

    public int GetCountTxtConfirms()
    {
        string[] stuff = txtConfirms.Split('*');
        return stuff.Length;
    }

    public void RemoveFromTxtConfirms(string txtRemove)
    {
        if (txtConfirms == "") return;
        string[] stuff = txtConfirms.Split('*');
        List<string> stuffNew = new List<string>();
        foreach (string txt in stuff)
        {
            if (txt.Contains(txtRemove) == false)
            {
                stuffNew.Add(txt);
            }
        }
        txtConfirms = String.Join("*", stuffNew);
        int n = expires.IndexOf(txtRemove);
        expires.RemoveAt(n);
        expireCycleCounts.RemoveAt(n);
    }

    void AddConfirms()
    {
        if (txtConfirms !=  "")
        {
            if (txtSend != "")
            {
                txtSend += "*";
            }
            txtSend += txtConfirms;
        }
    }

    public void AddCreateSendKeyValueToTxtConfirmsExpires(string txt)
    {
        expires.Add(txt);
        expireCycleCounts.Add(0);
        AddCreateSendKeyValueToTxtConfirms(txt);
    }

    public void AddCreateSendKeyValueToTxtConfirms(string txt)
    {
        if (txtConfirms != "")
        {
            txtConfirms += "*";
        }
        txtConfirms += txt;
    }

    public void UpdateExpiredFromTxtConfirms()
    {
        string[] stuff = txtConfirms.Split('*');
        foreach(string txt in stuff)
        {
            if (expires.Contains(txt) == true)
            {
                int n = expires.IndexOf(txt);
                if (expireCycleCounts[n] > expireCycleCountLimit)
                {
                    RemoveFromTxtConfirms(txt);
                } else
                {
                    expireCycleCounts[n]++;
                }
            }
        }
    }

    public void ClearConfirms()
    {
        txtConfirms = "";
    }

    string Vector3ToStringGoNtarget(int n)
    {
        GameObject go = g.goAssets.transform.GetChild(n).gameObject;
        string txt = go.name;
        txt += "|" + Vector3ToString(g.smoothMgr.posTargets[n]);
        txt += "|" + Vector3ToString(g.smoothMgr.eulTargets[n]);
        txt += "|" + Vector3ToString(g.smoothMgr.scaTargets[n]);
        return txt;
    }

    void ReceiveOtherNs()
    {
//        g.DebugLog("Receiving " + txtReceived);
        if (txtReceived.Contains("*") == false)
        {
            StringToGlobalsN();
            ReceiveOtherN();
            return;
        }
        string[] stuff = txtReceived.Split('*');
        foreach (string txt in stuff)
        {
            txtReceived = txt;
            StringToGlobalsN();
            ReceiveOtherN();
        }
    }

    void ReceiveOtherN()
    {
        if (nameReceived == "keyValue")
        {
            switch (keyReceived)
            {
                case "startTimeApp":
                    int startTimeOther = g.udpMgr.IntParse(valueReceived);
                    if (startTimeOther < secondsSince1970)
                    {
                        g.role = RoleType.receiver;
                    }
                    else
                    {
                        g.role = RoleType.sender;
                    }
                    SetConnected(true);
                    break;
                case "AddPaint":
                    g.sculptMgr.AddPaintRemote(valueReceived);
                    break;
                case "AddLine":
                    g.sculptMgr.AddLineRemote(valueReceived);
                    break;
                case "Advance":
                    g.assetMgr.AddAdvanceRemote(valueReceived);
                    break;
                case "AddNoise":
                    g.noiseMgr.AddNoiseRemote(valueReceived);
                    break;
                default:
                    Debug.Log("!keyValue " + keyReceived + ":" + valueReceived + "\n");    
                    break;
            }
        }
        else
        {
            UpdateMaxPayload(data);
            GameObject go = g.assetMgr.GetChildByName(g.goAssets, nameReceived);
            if (go != null)
            {
                g.smoothMgr.UpdateAssetPosTarget(go, posReceived);
                g.smoothMgr.UpdateAssetEulTarget(go, eulReceived);
                g.smoothMgr.UpdateAssetScaTarget(go, scaReceived);
                //
                g.smoothMgr.UpdateAssetPosTargetLast(go, posReceived);
                g.smoothMgr.UpdateAssetEulTargeLast(go, eulReceived);
                g.smoothMgr.UpdateAssetScaTargetLast(go, scaReceived);
            }
        }
    }

    void SendStartTimeApp()
    {
        SendKeyValue("startTimeApp", secondsSince1970.ToString());
    }

    public void SendThisN(int n)
    {
        string txt = Vector3ToStringGoN(n);
        SendString(txt);
    }

    void StartUdp()
    {
        DebugLog("Start Udp");
        remoteEndPointSender = new IPEndPoint(IPAddress.Broadcast, port);
        InitReceiveUDP();
    }

    void UpdateMaxPayload(byte[] data0)
    {
        if (data0.Length > maxPayload)
        {
            maxPayload = data0.Length;
        }
    }

    int IntParse(string txt)
    {
        int result;
        if (int.TryParse(txt, out result) == false)
        {
            g.DebugLog("? int parse " + txt);
            return -1;
        }
        return result;
    }

    void StringToGlobalsN()
    {
        string[] stuff = txtReceived.Split('|');
        nameReceived = stuff[0];
        if (nameReceived == "keyValue")
        {
            string[] stuff2 = stuff[1].Split(':');
            keyReceived = stuff2[0];
            if (stuff2.Length > 1)
            {
                valueReceived = stuff2[1];
            }
            else
            {
                valueReceived = null;
            }
        }
        else
        {
            if (stuff.Length >= 4)  
            {
                posReceived = StringToVector3(stuff[1]);
                eulReceived = StringToVector3(stuff[2]);
                scaReceived = StringToVector3(stuff[3]);
            }
        }
    }

    public string CreateSendKeyValue(string txtKey, string txtValue)
    {
        string txt = txtKey;
        if (string.IsNullOrEmpty(txtValue) == false)
        {
            txt += ":" + txtValue;
        }
        txt = "keyValue|" + txt;
        return txt;
    }

    public void SendKeyValue(string txtKey, string txtValue)
    {
        string txt = CreateSendKeyValue(txtKey, txtValue);
        SendString(txt);
    }

    string Vector3ToStringGoN(int n)
    {
        GameObject go = g.goAssets.transform.GetChild(n).gameObject;
        string txt = go.name;
        txt += "|" + Vector3ToString(go.transform.localPosition);
        txt += "|" + Vector3ToString(go.transform.localEulerAngles);
        txt += "|" + Vector3ToString(go.transform.localScale);
        return txt;
    }

    void InitReceiveUDP()
    {
        Application.runInBackground = true;
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
        DebugLog("listening on port " + port);
    }

    void SendString(string txt)
    {
        if (client == null) return;
        g.DebugLog("Sending " + txt);
        int nNum = txt.Length / maxDataSegmentLength;
        if (txt.Length > nNum * maxDataSegmentLength)
        {
            nNum++;
        }
        for (int n = 1; n <= nNum; n++)
        {
            int s = (n - 1) * maxDataSegmentLength;
            int l = maxDataSegmentLength;
            if (s + l > txt.Length)
            {
                l = txt.Length - s;
            }
            string txtSegment = secondsSince1970.ToString() + "|" + n + "/" + nNum + "|" + txt.Substring(s, l);
            data = System.Text.Encoding.ASCII.GetBytes(txtSegment);
            client.Send(data, data.Length, remoteEndPointSender);
            bytesSent += data.Length;
            UpdateMaxPayload(data);
            cntFrameSends++;
        }
    }

    void ReceiveData()
    {
        client = new UdpClient(port);
        while (true)
        {
            try
            {
                IPEndPoint anyIp = new IPEndPoint(IPAddress.Any, port);
                data = client.Receive(ref anyIp);
                string txt = System.Text.Encoding.ASCII.GetString(data);
                string[] stuff = txt.Split('|');
                if (stuff.Length > 0)
                {
                    secondsSince1970other = g.udpMgr.IntParse(stuff[0]);
                    if (secondsSince1970other != secondsSince1970)
                    {
                        bytesReceived += data.Length;
                        ipFrom = anyIp.Address.ToString();
                        txt = txt.Substring(stuff[0].Length + 1);
                        stuff = txt.Split('|');
                        UpdateNdataSegment(stuff[0]);
                        txt = txt.Substring(stuff[0].Length + 1);
                        if (nDataSegment == 1)
                        {
                            txtReceived = "";
                        }
                        txtReceived += txt;
                        if (nDataSegment == nDataSegmentNum)
                        {
                            ynReceived = true;
                        }
                    }
                }
            }
            catch (Exception err)
            {
                ynError = true;
                txtError = err.Message;
            }
        }
    }

    void UpdateNdataSegment(string txt)
    {
        string[] stuff = txt.Split('/');
        nDataSegment = g.udpMgr.IntParse(stuff[0]);
        nDataSegmentNum = g.udpMgr.IntParse(stuff[1]);
    }

    Vector3 StringToVector3(string txt)
    {
        string[] stuff = txt.Split(',');
        if (stuff.Length != 3) return Vector3.zero;
        float x = FloatParse(stuff[0]);
        float y = FloatParse(stuff[1]);
        float z = FloatParse(stuff[2]);
        return new Vector3(x, y, z);
    }

    public float FloatParse(string txt)
    {
        float result;
        if (float.TryParse(txt, out result) == false)
        {
            g.DebugLog("? float parse " + txt);
            return -1;
        }
        return result;
    }

    string Vector3ToString(Vector3 pos)
    {
        return pos.x.ToString(g.f8) + "," + pos.y.ToString(g.f8) + "," + pos.z.ToString(g.f8);
    }

    string GetIpLocal()
    {
        IPHostEntry host;
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return "?";
    }

    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }

    int GetSecondsSince1970()
    {
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        int cur_time = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
        return cur_time;
    }

    void DebugLog(string txt)
    {
        //Debug.Log(txt + "\n");
    }

    void FinishUdp()
    {
        Debug.Log("Finish Udp\n");
        if (client != null)
        {
            client.Close();
            client = null;
        }
        if (receiveThread != null)
        {
            receiveThread.Abort();
            receiveThread = null;
        }
    }

    private void OnDisable()
    {
        FinishUdp();
    }
}