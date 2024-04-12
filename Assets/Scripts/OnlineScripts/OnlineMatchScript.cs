using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Match;

public class OnlineMatchScript : NetworkBehaviour
{
    public GameObject lobby;
    public NetManScript _netScript;
    public GameObject _playerPrefab;
    public AudioSource _as;
    public AudioSource _asSFX;
    public AudioClip[] sfx;
    public AudioClip[] bm;
    int[] matchInfo;
    GameObject p1;
    GameObject p2;
    GameObject currentWorld;
    public Text p1winsText;
    public Text p2winsText;
    public KnockbackScript _ks;
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

    public void playSFX(int sfxID)
    {
        _asSFX.PlayOneShot(sfx[sfxID]);
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

        p1winsText.text = "Player one wins: " + PlayerPrefs.GetInt("P1 Points");
        p2winsText.text = "Player two wins: " + PlayerPrefs.GetInt("P2 Points");
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
}
