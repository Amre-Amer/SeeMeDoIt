using UnityEngine;

public class HitPlaceMgr : MonoBehaviour
{
    GlobalsMgr g;
    int touchCountLast;
    int touchCount;
    Vector3 eulRotate;
    Vector3 scrRotate;
    Vector3 posTargetGround;
    const float smooth = .1f;
    const float factor = .25f;

    private void Awake()
    {
        g = GetComponent<GlobalsMgr>();
    }

    // Start is called before the first frame update
    void Start()
    {
//        g.ynDetect = true;
        g.goProtractor.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        touchCount = g.touchMouseMgr.GetTouchMouseCount();
        UpdateHitPlace();
        UpdateSmoothPivot();
        UpdateAssetsGround();
        UpdateProtractor();
        touchCountLast = touchCount;
    }

    void UpdateProtractor()
    {
        if (g.mode == ModeType.play) return;
        if (g.ynRotateGround == false) return;
        g.goProtractor.transform.position = g.goGround.transform.position + g.goGround.transform.up * -1 * g.goGroundBase.transform.localScale.y;
        g.goProtractor.transform.eulerAngles = eulRotate;
    }

    void UpdateAssetsGround()
    {
        if (g.mode == ModeType.play) return;
        g.goAssets.transform.position = g.goGround.transform.position;
        g.goAssets.transform.eulerAngles = g.goGround.transform.eulerAngles;
        g.goAssets.transform.localScale = g.goGround.transform.localScale;
    }

    void UpdateSmoothPivot()
    {
        if (g.mode == ModeType.play) return;
        if (g.ynDetect == false) return;
        Vector3 pos = g.goGround.transform.position;
        pos = smooth * posTargetGround + (1 - smooth) * pos;
        g.goGround.transform.position = pos;
    }

    void UpdateHitPlace()
    {
        RaycastHit hit = new RaycastHit();
        if (touchCountLast != touchCount)
        {
            if (touchCount == 1)
            {
                if (g.ynDetect == false && g.ynRotateGround == false)
                {
                    Vector3 scr = g.touchMouseMgr.GetTouchMouseScrPos();
                    Ray ray = Camera.main.ScreenPointToRay(scr);
                    Physics.Raycast(ray, out hit, 10, g.layerPlane);
                    if (hit.transform != null)
                    {
                        g.ynDetect = true;
                        //g.modeMgr.UpdateMode();
                    }
                    RaycastHit hitPivot = new RaycastHit();
                    Physics.Raycast(ray, out hitPivot, 10, g.layerGroundPivot);
                    if (hitPivot.transform != null)
                    {
                        RaycastHit hitContent = new RaycastHit();
                        Physics.Raycast(ray, out hitContent, 10, g.layerContent);
                        if (hitContent.transform == null)
                        {
                            g.ynDetect = true;
                            //g.modeMgr.UpdateMode();
                        }
                    }
                } else
                {
                    g.ynDetect = false;
                    g.ynRotateGround = true;
                    g.goProtractor.SetActive(g.ynRotateGround);
                    scrRotate = g.touchMouseMgr.GetTouchMouseScrPos();
                    eulRotate = g.goGround.transform.eulerAngles;
                }
            } else
            {
                g.ynRotateGround = false;
                g.goProtractor.SetActive(g.ynRotateGround);
                //g.modeMgr.UpdateMode();
            }
        }
        else
        { 
            if (g.ynDetect == true)
            {
                if (g.goPlaneFinder.transform.childCount > 0)
                {
                    GameObject goPlaneIndicator = g.goPlaneFinder.transform.GetChild(0).gameObject;
                    posTargetGround = goPlaneIndicator.transform.position + g.goGround.transform.up * g.goGroundBase.transform.localScale.y;
                    g.goGround.transform.eulerAngles = goPlaneIndicator.transform.eulerAngles;
                }
            }
            if (touchCount == 1 && g.ynRotateGround == true)
            {
                float dYaw = (g.touchMouseMgr.GetTouchMouseScrPos().x - scrRotate.x) * factor;
                g.goGround.transform.eulerAngles = eulRotate + new Vector3(0, -dYaw, 0);
            }
        }
    }
}
