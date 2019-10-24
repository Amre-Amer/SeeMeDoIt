using UnityEngine;

public class RoleMgr : MonoBehaviour
{
    GlobalsMgr g;
    RoleType roleLast;

    private void Awake()
    {
        g = GetComponent<GlobalsMgr>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMode();
        roleLast = g.role;
    }

    void UpdateMode()
    {
        if (g.role != roleLast)
        {
            if (g.role == RoleType.sender)
            {
                //g.ynAuto = true;
            }
            if (g.role == RoleType.receiver)
            {
                g.ynAuto = false;
            }
        }
    }
}
