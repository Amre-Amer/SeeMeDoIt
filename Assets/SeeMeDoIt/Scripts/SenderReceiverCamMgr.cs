using UnityEngine;

public class SenderReceiverCamMgr : MonoBehaviour
{
    GlobalsMgr g;
    const float tolerance = .001f;
    float ang;
    bool ynConnectedLast;
    public bool ynDemo;

    private void Awake()
    {
        g = GetComponent<GlobalsMgr>();
    }

    private void Start()
    {
        ynDemo = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (g.mode == ModeType.place) return;
        UpdateDemo();
        UpdateSenderReceiverCam();
        ynConnectedLast = g.udpMgr.ynConnected;
    }

    void UpdateDemo()
    {
        if (ynDemo == false) return;
        GameObject goCam = g.camMain.gameObject;
        goCam.transform.position = g.goGround.transform.position;
        goCam.transform.position += g.goGround.transform.up * .5f;
        goCam.transform.eulerAngles = new Vector3(0, ang, 0);
        if (g.role == RoleType.sender)
        {
            goCam.transform.position += goCam.transform.forward * -1.25f;
        } else
        {
            goCam.transform.position += goCam.transform.forward * -1.75f;
        }
        goCam.transform.LookAt(g.goGround.transform.position);
        if (g.role == RoleType.sender)
        {
            ang += .35f;
        }
        else
        {
            ang -= .2f;
        }
    }

    void UpdateSenderReceiverCam()
    {
        if (g.udpMgr.ynConnected != ynConnectedLast)
        {
            if (g.udpMgr.ynConnected == true)
            {
                if (g.role == RoleType.sender)
                {
                    TurnOnOffSenderReceiverCams(false, true);
                }
                else
                {
                    TurnOnOffSenderReceiverCams(true, false);
                }
            }
            else
            {
                TurnOnOffSenderReceiverCams(false, false);
            }
        }
        if (g.udpMgr.ynConnected == true)
        {
            GameObject goCam =  SelectActiveCam();
            Vector3 posLocal = g.goGround.transform.InverseTransformPoint(g.camMain.transform.position);
            g.smoothMgr.UpdateAssetPosTarget(goCam, posLocal);
            g.smoothMgr.UpdateAssetEulTarget(goCam, g.camMain.transform.localEulerAngles);
            g.smoothMgr.UpdateAssetScaTarget(goCam, g.camMain.transform.localScale);
            //
            TurnOnOffActiveCam();
        } else
        {
            TurnOnOffSenderReceiverCams(false, false);
        }
    }

    void TurnOnOffActiveCam()
    {
        if (g.role == RoleType.sender)
        {
            TurnOnOffSenderReceiverCams(false, true);
        }
        else
        {
            TurnOnOffSenderReceiverCams(true, false);
        }
    }

    GameObject SelectActiveCam()
    {
        GameObject goCam = null;
        if (g.role == RoleType.sender)
        {
            goCam = g.goSenderCam;
        }
        else
        {
            goCam = g.goReceiverCam;
        }
        return goCam;
    }

    void TurnOnOffSenderReceiverCams(bool ynSender, bool ynReceiver)
    {
        if (g.goSenderCam != null)
        {
            g.goSenderCam.SetActive(ynSender);
        }
        if (g.goReceiverCam != null)
        {
            g.goReceiverCam.SetActive(ynReceiver);
        }
    }
}
