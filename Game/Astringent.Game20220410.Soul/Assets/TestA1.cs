using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestA1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<UnityEngine.Rigidbody>().velocity = Vector3.left;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
