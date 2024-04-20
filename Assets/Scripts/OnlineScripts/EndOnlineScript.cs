using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EndOnlineScript : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (IsServer)
        {
            NetworkManager.Shutdown();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
