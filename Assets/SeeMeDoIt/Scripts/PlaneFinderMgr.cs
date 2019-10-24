using UnityEngine;
using Vuforia;

public class PlaneFinderMgr : MonoBehaviour
{
    GlobalsMgr g;

    private void Awake()
    {
        g = GetComponent<GlobalsMgr>();
    }

    public void PlaneFinderOnOff(bool yn)
    {
        if (g.goPlaneFinder == null)
        {
            Debug.Log("!plane finder game object");
            return;
        }
        PlaneFinderBehaviour planeFinder = g.goPlaneFinder.GetComponent<PlaneFinderBehaviour>();
        if (planeFinder != null)
        {
            planeFinder.enabled = yn;
        }
        else
        {
            Debug.Log("!plane finder behavior");
        }
    }
}
