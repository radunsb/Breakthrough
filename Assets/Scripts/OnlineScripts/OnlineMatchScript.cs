using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;
using System.Globalization;
using System;

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
    bool gameHasStarted;

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

    private void Update()
    {
        if (gameHasStarted && !checkFor2Players())
        {
            if (p1Score.Value < 3 && p2Score.Value < 3)
            {
                SceneManager.LoadScene("DisconnectScene");
            }
            else
            {
                SceneManager.LoadScene("WinScene");
            }
        }
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
        if (!gameHasStarted)
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
                foreach (GameObject player in players)
                {
                    if (player.GetComponent<OnlinePlayerScript>().IsLocalPlayer)
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
            p1 = playerOne;
            p2 = playerTwo;
            p1.GetComponent<PlayerScript>().playerIndex = 0;
            p2.GetComponent<PlayerScript>().playerIndex = 1;
            gameHasStarted = true;
            p1winsText.text = "Player one wins: " + p1Score.Value;
            p2winsText.text = "Player two wins: " + p2Score.Value;
            //Set gameObjects for each entity           
            matchInfo[2] = PlayerPrefs.GetInt("Stage");
            playMusic(matchInfo[2]);
            icons[p1.GetComponent<OnlinePlayerScript>().characterType].SetActive(true);
            icons[p2.GetComponent<OnlinePlayerScript>().characterType + 4].SetActive(true);
            p1winsText.color = Color.cyan;
            p2winsText.color = new Color(1f, 0.4f, 0f);
            p1.GetComponent<PlayerScript>().triangle.GetComponent<SpriteRenderer>().color = Color.cyan;
            p2.GetComponent<PlayerScript>().triangle.GetComponent<SpriteRenderer>().color = new Color(1f, 0.4f, 0f);
        }
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

    [ClientRpc]
    public void freezePlayersClientRpc()
    {
        p1.GetComponent<OnlinePlayerScript>().freeze();
        p2.GetComponent<OnlinePlayerScript>().freeze();
        
    }

    [ServerRpc (RequireOwnership = false)]
    public void freezePlayersServerRpc()
    {
        p1.GetComponent<OnlinePlayerScript>().freeze();
        p2.GetComponent<OnlinePlayerScript>().freeze();

    }

    [ClientRpc]
    public void unfreezePlayersClientRpc()
    {
        p1.GetComponent<Rigidbody2D>().gravityScale = 1.8f;
        p2.GetComponent<Rigidbody2D>().gravityScale = 1.8f;
    }

    [ServerRpc (RequireOwnership = false)]
    public void unfreezePlayersServerRpc()
    {
        p1.GetComponent<Rigidbody2D>().gravityScale = 1.8f;
        p2.GetComponent<Rigidbody2D>().gravityScale = 1.8f;
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
        //Despawn the network object for the old arena and spawn in the new one
        oldWorld.GetComponent<NetworkObject>().Despawn();
        Destroy(oldWorld);
        destroyBackgroundClientRpc("Old World");
        GameObject newWorld = Instantiate(currentWorld);
        newWorld.GetComponent<NetworkObject>().Spawn();
        currentWorld = newWorld;
        
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
        if (IsServer)
        {
            StartCoroutine(stalling());           
        }
        SceneManager.LoadScene("WinScene");
    }

    IEnumerator stalling()
    {
        yield return new WaitForSeconds(2);
        NetworkManager.Shutdown();
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
