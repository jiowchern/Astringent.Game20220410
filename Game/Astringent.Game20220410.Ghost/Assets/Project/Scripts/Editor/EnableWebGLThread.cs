using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class EnableWebGLThread : MonoBehaviour
{

    static EnableWebGLThread()
    {

        PlayerSettings.WebGL.threadsSupport = false;
       Debug.Log("Up and running");
    }
   
}
