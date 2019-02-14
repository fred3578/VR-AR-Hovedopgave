using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BrickID : NetworkBehaviour
{
    [SyncVar] public int ID;
    //[SyncVar] public Vector3 BrickPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    
}
