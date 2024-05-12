using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Net;
using Unity.Netcode.Transports.UTP;

public class NetManScript : MonoBehaviour
{
    public OnlineMatchScript _oms;
    private void Awake()
    {
        _oms = GameObject.FindObjectOfType<OnlineMatchScript>();
    }
    public void OnStartHost()
    {
        UnityTransport transport =
        (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        transport.SetConnectionData(PlayerPrefs.GetString("targetIP"), 7777);
        NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;
        NetworkManager.Singleton.ConnectionApprovalCallback = ConnectionCallback;
        NetworkManager.Singleton.StartHost();
    }

    public void OnStartClient()
    {
        UnityTransport transport =
        (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        transport.SetConnectionData(PlayerPrefs.GetString("targetIP"), 7777);
        NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;
        NetworkManager.Singleton.ConnectionApprovalCallback = ConnectionCallback;
        NetworkManager.Singleton.StartClient();
        StartCoroutine(waitForOpponent());
        
    }
    void ConnectionCallback(NetworkManager.ConnectionApprovalRequest request,
NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = true;
        response.CreatePlayerObject = false;
    }

    public IEnumerator waitForOpponent()
    {
        int checks = 0;
        while (checks < 10)
        {
            yield return new WaitForSeconds(.5f);
            if (_oms.checkFor2Players())
            {
                break;
            }
            else
            {
                checks++;
            }
        }
        if (!_oms.checkFor2Players())
        {
            NetworkManager.Singleton.Shutdown();
            StartCoroutine(shutdownClient());
        }
    }

    public IEnumerator shutdownClient()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (!NetworkManager.Singleton.ShutdownInProgress)
            {
                OnStartHost();
                break;
            }
        }
    }
}
