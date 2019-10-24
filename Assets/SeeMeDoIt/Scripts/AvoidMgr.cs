using UnityEngine;

public class AvoidMgr : MonoBehaviour
{
    GlobalsMgr g;
    const float distNear = .09f;
    GameObject goCheck;
    GameObject goNear;

    private void Awake()
    {
        g = GetComponent<GlobalsMgr>();
    }

    // Update is called once per frame
    void Update()
    {
        if (g.mode == ModeType.place) return;
        if (g.ynAssemble == true) return;
        UpdateAvoid();
    }

    void UpdateAvoid()
    {
        foreach(Transform t in g.goAssets.transform)
        {
            if (IsAvoidable(t.gameObject) == true)
            {
                goCheck = t.gameObject;
                Avoid();
            }
        }
    }

    void Avoid()
    {
        FindNear();
        AvoidNear();
    }

    void FindNear()
    {
        goNear = null;
        foreach (Transform t in g.goAssets.transform)
        {
            if (goCheck != t.gameObject)
            {
                if (IsAvoidable(t.gameObject) == true)
                {
                    float dist = Vector3.Distance(goCheck.transform.position, t.position);
                    if (dist < distNear)
                    {
                        goNear = t.gameObject;
                        return;
                    }
                }
            }
        }
    }

    bool IsAvoidable(GameObject go)
    {
        if (go == g.goPointer || go == g.goSenderCam || go == g.goReceiverCam)
        {
            return false;
        }
        if (g.smoothMgr.IsPaintOrLine(go) == true)
        {
            return false;
        }
        return true;
    }

    void AvoidNear()
    {
        if (goNear == null) return;
        Vector3 vector = goCheck.transform.position - goNear.transform.position;
        Vector3 pos = goNear.transform.position + vector.normalized * distNear;
        //if (pos.y < 0)
        if (goCheck == g.goAsset)
        {
            Vector3 posLocal = g.goGround.transform.InverseTransformPoint(pos); // amre
            g.smoothMgr.UpdateAssetPosTarget(g.goAsset, posLocal);
        } else
        {
            Vector3 posL = g.goAssets.transform.InverseTransformPoint(pos);
            if (posL.y < 0)
            {
                posL.y = 0;
                pos = g.goAssets.transform.TransformPoint(posL);
            }
            goCheck.transform.position = pos;
        }
    }
}
