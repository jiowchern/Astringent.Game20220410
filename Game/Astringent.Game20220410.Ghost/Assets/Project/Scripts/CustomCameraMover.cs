using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CustomCameraMover : MonoBehaviour
{
    void Start()
    {
        CinemachineCore.GetInputAxis = GetAxisCustom;
    }
    public float GetAxisCustom(string axisName)
    {
        if (axisName == "Mouse X")
        {
            var val = UnityEngine.Input.GetAxis("Mouse X");
            
            if(val == 0)
                return 0;
            if (Input.GetMouseButton(1)  )
            {
                
                return val;
            }
            else
            {
                return 0;
            }
        }
       
        return UnityEngine.Input.GetAxis(axisName);
    }
}
