using UnityEngine;

public class BoundsMgr : MonoBehaviour
{
    GlobalsMgr g;

    private void Awake()
    {
        g = GetComponent<GlobalsMgr>();
    }

    private void Start()
    {
        UpdateBoundingBox();
    }

    private void Update()
    {
        if (g.mode == ModeType.place) return;
        if (g.goAsset == null) return;
        UpdateBoundingBox(); 
    }

    public void UpdateBoundingBox()
    {
        g.goBoundingBox.SetActive(g.ynShowBoundingBox);
        if (g.ynShowBoundingBox == true)
        {
            g.boundingBox = GetBounds(g.goAsset);
            UpdateGoBoundingBox(g.goAsset, g.boundingBox, g.goBoundingBox);
        }
    }

    public void UpdateGoBoundingBox(GameObject go, Bounds bb, GameObject goBB)
    {
        goBB.transform.position = go.transform.TransformPoint(bb.center);
        goBB.transform.eulerAngles = go.transform.eulerAngles;
        goBB.transform.localScale = bb.extents * 2;
    }

    public Bounds GetBounds(GameObject go)
    {
        Bounds bb = new Bounds(Vector3.zero, Vector3.zero);
        Vector3 eul = go.transform.eulerAngles;
        go.transform.eulerAngles = Vector3.zero;
        //
        Vector3 pos = go.transform.position;
        go.transform.position = Vector3.zero;
        //
        MeshRenderer[] mrs = go.GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer mr in mrs)
        {
            bb.Encapsulate(mr.bounds);
        }
        //
        go.transform.eulerAngles = eul;
        go.transform.position = pos;
        //
        return bb;
    }
}
