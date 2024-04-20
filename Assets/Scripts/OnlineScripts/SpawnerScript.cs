using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpawnerScript : NetworkBehaviour
{
    public GameObject[] prefabs;
    GameObject player;
    public override void OnNetworkSpawn()
    {
        CreatePlayerServerRpc(NetworkManager.Singleton.LocalClientId, PlayerPrefs.GetInt("Player1Char"));
    }

    [ServerRpc(RequireOwnership = false)]
    void CreatePlayerServerRpc(ulong clientId, int playerId)
    {
        player = Instantiate(prefabs[playerId]);
        NetworkObject netObj = player.GetComponent<NetworkObject>();
        player.SetActive(true);
        netObj.SpawnAsPlayerObject(clientId, true);
    }
}
