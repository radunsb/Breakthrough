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
    public GameObject currentWorld;
    public GameObject oldWorld;

    NetworkVariable<int> p1Score = new NetworkVariable<int>();
    NetworkVariable<int> p2Score = new NetworkVariable<int>();
    // Start is called before the first frame update
    void Start()
    {
        _netScript.OnStartClient();
        StartCoroutine(waitForSecondPlayer());
        p1Score.OnValueChanged += updateP1Points;
        p2Score.OnValueChanged += updateP2Points;
    }

    public void updateP1Points(int previous, int current)
    {
        p1Score.Value = current;
        p1winsText.text = "Player one wins: " + p1Score.Value;
    }

    public void updateP2Points(int previous, int current)
    {
        p2Score.Value = current;
        p2winsText.text = "Player two wins: " + p2Score.Value;
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
            playerTwo = players[0];
            foreach(GameObject player in players)
            {
                if(player.GetComponent<OnlinePlayerScript>().IsLocalPlayer)
                {
                    playerOne = player;
                }
                else
                {
                    playerTwo = player;
                }
            }
            playerOne.transform.position = new Vector2(-4, -2);
            playerTwo.transform.position = new Vector2(4, -2);
            destroyBackgroundClientRpc("Lobby");
        }
        else
        {
            playerOne = players[0];
            playerTwo = players[0];
            foreach (GameObject player in players)
            {
                if (player.GetComponent<OnlinePlayerScript>().IsLocalPlayer)
                {
                    playerTwo = player;
                }
                else
                {
                    playerOne = player;
                }
            }
        }

        p1winsText.text = "Player one wins: " + p1Score.Value;
        p2winsText.text = "Player two wins: " + p2Score.Value;
        //Set gameObjects for each entity
        p1 = playerOne;
        p2 = playerTwo;
        matchInfo[2] = PlayerPrefs.GetInt("Stage");
        playMusic(matchInfo[2]);
    }

    [ClientRpc]
    void destroyBackgroundClientRpc(string toDestroy)
    {
        print("Made it to destoryBackgroundClientRpc");
        if(toDestroy == "Lobby")
        {
            Destroy(lobby);
        }
        else if(toDestroy == "Current World")
        {
            Destroy(currentWorld);
        }
        else if(toDestroy == "Old World")
        {
            Destroy(oldWorld);
        }
    }

    [ServerRpc]
    public void updatePointsServerRpc(int playerIndex)
    {
        //Set NetworkVariables based off winner of round
        if(playerIndex == 0)
        {
            p1Score.Value = p1Score.Value + 1;
        }
        else
        {
            p2Score.Value = p2Score.Value + 1;
        }
        //Determine if someone just won the match and quit out if so
        if(p1Score.Value >= 3)
        {
            matchOverClientRpc(0);
        }
        else if(p2Score.Value >= 3)
        {
            matchOverClientRpc(1);
        }
        //otherwise initiate the new round
        else
        {
            roundOverServerRpc();           
        }
    }   

    [ServerRpc]
    void roundOverServerRpc()
    {
        print("Made it to roundOverServerRpc");
        //Despawn the network object for the old arena and spawn in the new one
        oldWorld.GetComponent<NetworkObject>().Despawn();
        Destroy(oldWorld);
        destroyBackgroundClientRpc("Old World");
        GameObject newWorld = Instantiate(currentWorld);
        newWorld.GetComponent<NetworkObject>().Spawn();
        currentWorld = newWorld;
        //Reset the player's positions server-side
        p1.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        p1.transform.position = new Vector2(-4, -2);
        p1.GetComponent<OnlineKnockbackScript>().charDamage.Value = 0;
        //Reset player two
        p2.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        p2.transform.position = new Vector2(4, -2);
        p2.GetComponent<OnlineKnockbackScript>().charDamage.Value = 0;
        //Reset the player's positions client-side
        //Clunky, but should work
        hardResetPositionsClientRpc();
    }

    //Necessary so that player doesn't get stuck outside of arena while trying to interpolate back to their starting position
    //(Easier to just set it manually)
    [ClientRpc]
    void hardResetPositionsClientRpc()
    {
        p1.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        p2.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        p1.transform.position = new Vector2(-4, -2);
        p2.transform.position = new Vector2(4, -2);
    }

    [ClientRpc]
    void matchOverClientRpc(int winningPlayerIndex)
    {
        
        PlayerPrefs.SetInt("P1 Points", p1Score.Value);
        PlayerPrefs.SetInt("P2 Points", p2Score.Value);
        endGame();       
    }

    void endGame()
    {
        if (IsServer)
        {
            NetworkManager.Shutdown();
            SceneManager.LoadScene("WinScene");
        }
    }

    //First function in chain started from boundary activation
    public override void updateCharacterPoints(int playerIndex, GameObject worldToSpawn)
    {
        print("Made it to updateCharacterPoints");
        //Should update the old in current worlds locally for both clients
        oldWorld = currentWorld;
        currentWorld = worldToSpawn;
        //Everything else should only be run once, owner of this match script tells server to start the chain
        if (IsOwner)
        {
            updatePointsServerRpc(playerIndex);
        }
    }
    
    public void setCurrentWorld(GameObject worldToSpawn)
    {
        currentWorld = worldToSpawn;
    }

}
