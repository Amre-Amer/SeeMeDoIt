using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchMouseMgr : MonoBehaviour
{
    List<Button> buttons = new List<Button>();

    public bool WasAButtonTouched()
    {
        bool yn = false;
        foreach(Button button in buttons)
        {
            if (WasButtonTouched(button) == true)
            {
                yn = true;
                break;
            }
        }
        return yn;
    }

    bool WasButtonTouched(Button button)
    {
        bool yn = false;
        if (GetTouchMouseCount() != 1) return yn;
        float rad = button.GetComponent<RectTransform>().sizeDelta.x;
        Vector3 scr = GetTouchMouseScrPos();
        float dist = Vector3.Distance(scr, button.transform.position);
        if (dist <  rad)
        {
            yn = true;
        }
        return yn;
    }

    public int GetTouchMouseCount()
    {
        if (Application.isEditor == true)
        {
            if (Input.GetMouseButton(0) == true)
            {
                if (Input.GetKey(KeyCode.LeftShift) == true)
                {
                    if (Input.GetKey(KeyCode.LeftAlt) == true)
                    {
                        if (Input.GetKey(KeyCode.LeftControl) == true)
                        {
                            return 4;
                        }
                        else
                        {
                            return 3;
                        }
                    }
                    else
                    {
                        return 2;
                    }
                }
                else
                {
                    return 1;
                }
            } else {
                return 0;
            }
        }
        else
        {
            return Input.touchCount;
        }
    }

    public Vector3 GetTouchMouseScrPosMid()
    {
        if (Application.isEditor == true)
        {
            return (Input.mousePosition + new Vector3(Screen.width / 2, Screen.height / 2, 0)) / 2;
        }
        else
        {
            return (Input.touches[0].position + Input.touches[1].position) / 2;
        }
    }

    public float GetTouchMouseScrRadThree()
    {
        Vector3 cen = GetCentroidThree();
        float dist1 = Vector3.Distance(GetTouchMouseScrPos(), cen);
        float dist2 = Vector3.Distance(GetTouchMouseSecondScrPos(), cen);
        float dist3 = Vector3.Distance(GetTouchMouseThirdScrPos(), cen);
        return (dist1 + dist2 + dist3) / 3;
    }

    Vector3 GetCentroidThree()
    {
        return (GetTouchMouseScrPos() + GetTouchMouseSecondScrPos() + GetTouchMouseThirdScrPos()) / 3;
    }

    public Vector3 GetTouchMouseScrPosMidThree()
    {
        if (Application.isEditor == true)
        {
            return (Input.mousePosition + new Vector3(Screen.width / 3, Screen.height / 3, 0)) / 2;
        }
        else
        {
            return (Input.touches[0].position + Input.touches[1].position + Input.touches[2].position) / 3;
        }
    }

    public Vector3 GetTouchMouseScrPos()
    {
        if (Application.isEditor == true)
        {
            return Input.mousePosition;
        }
        else
        {
            return Input.touches[0].position;
        }
    }

    public Vector3 GetTouchMouseSecondScrPos()
    {
        if (Application.isEditor == true)
        {
            return new Vector3(Screen.width / 2, Screen.height / 2, 0);
        }
        else
        {
            return Input.touches[1].position;
        }
    }

    public Vector3 GetTouchMouseThirdScrPos()
    {
        if (Application.isEditor == true)
        {
            return new Vector3(Screen.width / 4, Screen.height / 4, 0);
        }
        else
        {
            return Input.touches[2].position;
        }
    }

    public Vector3 GetTouchMouseFourthScrPos()
    {
        if (Application.isEditor == true)
        {
            return new Vector3(Screen.width * 3 / 4, Screen.height* 3 / 4, 0);
        }
        else
        {
            return Input.touches[3].position;
        }
    }

    public float GetDistTouchesFour()
    {
        if (Application.isEditor == true)
        {
            if (Input.GetKey(KeyCode.LeftShift) == true)
            {
                Vector3 scrMid = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                return Vector3.Distance(Input.mousePosition, scrMid);
            }
            else
            {
                return -1;
            }
        }
        else
        {
            //            return Vector3.Distance(Input.touches[0].position, Input.touches[1].position);
            return GetRadiusFourTouches();
        }
    }

    float GetRadiusFourTouches()
    {
        Vector2 tot = Vector2.zero;
        for(int n = 0; n < Input.touches.Length; n++)
        {
            Touch touch = Input.touches[n];
            tot += touch.position;
        }
        Vector3 center = tot / Input.touches.Length;
        float totDist = 0;
        for (int n = 0; n < Input.touches.Length; n++)
        {
            Touch touch = Input.touches[n];
            float dist = Vector2.Distance(touch.position, center);
            totDist += dist;
        }
        return totDist / Input.touches.Length;
    }

    public float GetDistTouches()
    {
        if (Application.isEditor == true)
        {
            if (Input.GetKey(KeyCode.LeftShift) == true)
            {
                Vector3 scrMid = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                return Vector3.Distance(Input.mousePosition, scrMid);
            }
            else
            {
                return -1;
            }
        }
        else
        {
            return Vector3.Distance(Input.touches[0].position, Input.touches[1].position);
        }
    }
}
