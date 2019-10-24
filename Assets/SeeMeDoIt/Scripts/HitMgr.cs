using UnityEngine;

public class HitMgr : MonoBehaviour
{
    GlobalsMgr g;
    int touchCountLast;
    int touchCount;

    private void Awake()
    {
        g = GetComponent<GlobalsMgr>();
    }

    // Update is called once per frame
    void Update()
    {
        if (g.mode == ModeType.place) return;
        touchCount = g.touchMouseMgr.GetTouchMouseCount();
        UpdateHit();
        touchCountLast = touchCount;
    }

    public bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }

    public bool IsHitInLayerMask(RaycastHit hit, LayerMask layermask)
    {
        return layermask == (layermask | (1 << hit.transform.gameObject.layer));
    }

    void UpdateHit()
    {
        if (touchCount != touchCountLast)
        {
            if (touchCount == 1)
            {
                g.goAsset = null;
                Vector3 scrAny = g.touchMouseMgr.GetTouchMouseScrPos();
                Ray rayAny = Camera.main.ScreenPointToRay(scrAny);
                RaycastHit hitAny = new RaycastHit();
                Physics.Raycast(rayAny, out hitAny, 10, g.layerAdvance | g.layerRetreat | g.layerCloud | g.layerContent | g.layerGround | g.layerTable);
                if (hitAny.transform != null)
                {
                    if (IsHitInLayerMask(hitAny, g.layerAdvance) == true)
                    {
                        g.assetMgr.ButtonAdvanceAssetsClicked();
                    }
                    else
                    {
                        if (IsHitInLayerMask(hitAny, g.layerRetreat) == true)
                        {
                            g.assetMgr.ButtonRetreatAssetsClicked();
                        }
                        else {
                            if (IsHitInLayerMask(hitAny, g.layerCloud) == true)
                            {
                                g.assetMgr.ButtonCloudAssetsClicked();
                            }
                            else
                            {
                                if (IsHitInLayerMask(hitAny, g.layerContent) == true)
                                {
                                    g.goAsset = g.assetMgr.FindAssetParent(hitAny.transform.gameObject);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (touchCount == 0)
                {
                    if (g.goAsset != null)
                    {
                        g.smoothMgr.UpdateAssetPosTarget(g.goAsset, g.goAsset.transform.localPosition);
                        g.goAsset = null;
                    }
                }
            }
        }
        else
        {
            if (touchCount == 1)
            {
                if (g.goAsset != null)
                {
                    RaycastHit hitGround = new RaycastHit();
                    Vector3 scr = g.touchMouseMgr.GetTouchMouseScrPos();
                    Ray ray = Camera.main.ScreenPointToRay(scr);
                    Physics.Raycast(ray, out hitGround, 10, g.layerGround);
                    if (hitGround.transform != null)
                    {
                        Vector3 posLocal = g.goGround.transform.InverseTransformPoint(hitGround.point); // amre
                        posLocal.y = 0;  // amre
                        g.smoothMgr.UpdateAssetPosTarget(g.goAsset, posLocal);
                    }
                }
            }
        }
    }
}
