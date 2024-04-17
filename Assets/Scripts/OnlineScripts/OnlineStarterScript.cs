using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class OnlineStarterScript : NetworkBehaviour
{
    public GameObject houseStarter;
    public GameObject subwayStarter;
    public GameObject towerStarter;
    OnlineMatchScript _oms;
    // Start is called before the first frame update
    void Start()
    {
        _oms = GameObject.FindObjectOfType<OnlineMatchScript>();
    }

    public void Initialize()
    {
        int actualStage = PlayerPrefs.GetInt("Stage");
        if (IsServer)
        {
            if(actualStage == 4)
            {
                GameObject st = Instantiate(houseStarter);
                var stNetObject = st.GetComponent<NetworkObject>();
                stNetObject.Spawn();
                _oms.setCurrentWorld(st);
            }
            else if(actualStage == 5)
            {
                GameObject st = Instantiate(subwayStarter);
                var stNetObject = st.GetComponent<NetworkObject>();
                stNetObject.Spawn();
                _oms.setCurrentWorld(st);
            }
            else
            {
                GameObject st = Instantiate(towerStarter);
                var stNetObject = st.GetComponent<NetworkObject>();
                stNetObject.Spawn();
                _oms.setCurrentWorld(st);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
