using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchScript : MonoBehaviour
{
    public List<GameObject> managers;
    public int roundsToWin;
    public GameObject[] possiblePlayers;
    public GameObject[] possibleSandbags;
    public GameObject[] possibleEnemies;
    public Text p1winsText;
    public Text p2winsText;
    void Start()
    {
        if(PlayerPrefs.GetString("Match Type") == "2 Player Local")
        {
            Instantiate(managers[0]);
            GameObject playerOne = Instantiate(possiblePlayers[PlayerPrefs.GetInt("Player1Char")],
            new Vector2(-4f, -3f), Quaternion.identity);
            GameObject playerTwo = Instantiate(possiblePlayers[PlayerPrefs.GetInt("Player2Char")],
                new Vector2(4f, -3f), Quaternion.identity);
            playerOne.GetComponent<PlayerScript>().playerIndex = 0;
            playerTwo.GetComponent<PlayerScript>().playerIndex = 1;
            p1winsText.text = "Player one wins: " + PlayerPrefs.GetInt("P1 Points");
            p2winsText.text = "Player two wins: " + PlayerPrefs.GetInt("P2 Points");
        }
        else if(PlayerPrefs.GetString("Match Type") == "Training")
        {
            Instantiate(managers[1]);
            GameObject playerOne = Instantiate(possiblePlayers[PlayerPrefs.GetInt("Player1Char")],
            new Vector2(-4f, -3f), Quaternion.identity);
            playerOne.GetComponent<PlayerScript>().playerIndex = 0;
            GameObject sandbagOne = Instantiate(possibleSandbags[PlayerPrefs.GetInt("Player2Char")],
                new Vector2(4f, -3f), Quaternion.identity);
        }
        
        
    }

    void Update()
    {
        // will eventually open a pause menu
        if (Input.GetKeyDown(KeyCode.Escape)){
            SceneManager.LoadScene("TitleScene");
        }
    }

    void updateCharacterPoints(int playerIndex)
    {
        int newPoints = (playerIndex == 0) ? PlayerPrefs.GetInt("P1 Points")
            : PlayerPrefs.GetInt("P2 Points") + 1;
        if(newPoints >= roundsToWin)
        {
            matchOver(playerIndex);
        }
        else
        {
            matchOver(playerIndex);
        }
    }

    void roundOver(int winningPlayerIndex)
    {
        SceneManager.LoadScene("MatchScene");
    }

    void matchOver(int winningPlayerIndex)
    {
        SceneManager.LoadScene("CSScene");
    }
}
