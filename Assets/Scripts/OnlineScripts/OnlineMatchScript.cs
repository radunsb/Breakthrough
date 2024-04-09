using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OnlineMatchScript : MonoBehaviour
{
    public NetManScript _netScript;
    public GameObject _playerPrefab;
    // Start is called before the first frame update
    void Start()
    {
        _netScript.OnStartClient();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool checkFor2Players()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 2)
        {
            return true;
        }
        return false;
    }
}
