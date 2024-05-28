using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_MaskObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshRenderer>().material.renderQueue = 3002;   
    }
}
