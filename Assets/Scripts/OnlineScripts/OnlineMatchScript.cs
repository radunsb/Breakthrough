using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;
using System.Globalization;

public class OnlineMatchScript : MatchScript
{
    public GameObject lobby;
    public NetManScript _netScript;
    public GameObject _playerPrefab;
    public AudioClip[] bm;
    GameObject p1;
    GameObject p2;
    GameObject currentWorld;
    public KnockbackScript _ks;

    NetworkVariable<int> p1Score = new NetworkVariable<int>();
    NetworkVariable<int> p2Score = new NetworkVariable<int>();
    // Start is called before the first frame update
    void Start()
    {
        _netScript.OnStartClient();
        StartCoroutine(waitForSecondPlayer());
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

    public void playMusic(int bmID)
    {
        _as.clip = bm[bmID - 4];
        _as.Play();
    }
    IEnumerator waitForSecondPlayer()
    {
        while (!checkFor2Players())
        {
            yield return new WaitForSeconds(1);
        }
        GetComponent<OnlineStarterScript>().Initialize();
        _as.volume = (PlayerPrefs.HasKey("Volume")) ? PlayerPrefs.GetFloat("Volume") : 1.0f;
        _asSFX.volume = (PlayerPrefs.HasKey("Volume")) ? PlayerPrefs.GetFloat("Volume") : 1.0f;
        matchInfo = new int[3];
        GameObject playerOne;
        GameObject playerTwo;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (IsServer)
        {
            playerOne = players[0];
            foreach(GameObject player in players)
            {
                if(player != gameObject)
                {
                    playerOne = player;
                }
            }
            playerTwo = gameObject;
            playerOne.transform.position = new Vector2(-4, -2);
            playerTwo.transform.position = new Vector2(4, -2);
            destroyBackgroundClientRpc("Lobby");
        }
        else
        {
            playerOne = gameObject;
            playerTwo = players[0];
            foreach (GameObject player in players)
            {
                if (player != gameObject)
                {
                    playerTwo = player;
                }
            }
        }

        p1winsText.text = "Player one wins: " + p1Score.Value;
        p2winsText.text = "Player two wins: " + p2Score.Value;
        //Set gameObjects for each entity
        p1 = playerOne;
        p2 = playerTwo;
        matchInfo[2] = PlayerPrefs.GetInt("Stage");
        //Should be updated for the other main stages
        if (matchInfo[2] == 4)
        {
            currentWorld = GameObject.Find("House (Main)");
        }
        else if (matchInfo[2] == 5)
        {
            currentWorld = GameObject.Find("Subway (Main)");
        }
        else
        {
            currentWorld = GameObject.Find("Tower (Main)");
        }
        playMusic(matchInfo[2]);
    }

    [ClientRpc]
    void destroyBackgroundClientRpc(string toDestroy)
    {
        if(toDestroy == "Lobby")
        {
            Destroy(lobby);
        }
        else if(toDestroy == "Current World")
        {
            Destroy(currentWorld);
        }
    }

    [ServerRpc]
    public void updatePointsServerRpc(int playerIndex)
    {
        if(playerIndex == 0)
        {
            p1Score.Value = p1Score.Value + 1;
        }
        else
        {
            p2Score.Value = p2Score.Value + 1;
        }
    }
    
    protected override IEnumerator roundOver(int winningPlayerIndex, GameObject worldToSpawn)
    {
        yield return new WaitForSeconds(1);
        roundOverServerRpc();      
        p1winsText.text = "Player one wins: " + p1Score.Value;
        p2winsText.text = "Player two wins: " + p2Score.Value;

    }

    [ServerRpc]
    void roundOverServerRpc()
    {
        currentWorld.GetComponent<NetworkObject>().Despawn();
        Destroy(currentWorld);
        destroyBackgroundClientRpc("Current World");
        p1.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        p1.transform.position = new Vector2(-4, -2);
        p1.GetComponent<OnlineKnockbackScript>().updateDamageServerRpc(0);
        //Reset player two
        p2.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        p2.transform.position = new Vector2(4, -2);
        p2.GetComponent<OnlineKnockbackScript>().updateDamageServerRpc(0);
    }

    void matchOver(int winningPlayerIndex)
    {
        SceneManager.LoadScene("WinScene");
    }

    public override void updateCharacterPoints(int playerIndex, GameObject worldToSpawn)
    {
        updatePointsServerRpc(playerIndex);
    }
}
