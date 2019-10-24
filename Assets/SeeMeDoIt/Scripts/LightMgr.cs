using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMgr : MonoBehaviour
{
    Light lt;
    int cntFrames;

    private void Awake()
    {
        lt = transform.Find("SpotLight").GetComponent<Light>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float spotAngle = 50 + 10 * Mathf.Sin(cntFrames * Mathf.Deg2Rad);
        lt.spotAngle = spotAngle;
        cntFrames++;
    }
}
