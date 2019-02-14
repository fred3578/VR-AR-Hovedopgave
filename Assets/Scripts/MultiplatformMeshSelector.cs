using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplatformMeshSelector : MonoBehaviour
{
   
    void Start()
    {
        if (Application.platform != RuntimePlatform.IPhonePlayer)
        {
            transform.Find("ARCoreMesh").gameObject.SetActive(true);
        }
        else
        {
            transform.Find("ARCoreMesh").gameObject.SetActive(false);
        }
    }

    void Update()
    {
        
    }
}
