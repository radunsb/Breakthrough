using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewCSScript : MonoBehaviour
{
    int[] matchInfo;
    public string state;
    public GameObject _cursorPrefab;
    public GameObject _readyButton;
    public Text statusText;
    public Text cpuText;
    string[] characters = {"", "REDMAN", "BLUEBLADE", "GREENBOW", "GOLDENFURY" };
    void Start()
    {
        
        int numPlayers = PlayerPrefs.GetString("Match Type") == "2 Player Local" ? 2 : 1;
        matchInfo = new int[] { -1, -1, -1 };
        state = "player select";
        if(numPlayers == 1)
        {
            GameObject p1Cursor = Instantiate(_cursorPrefab, new Vector2(-8, -2), Quaternion.identity);
            p1Cursor.GetComponent<SpriteRenderer>().color = Color.red;
            matchInfo[1] = 0;
            statusText.text = "vs REDMAN";
        }
        else
        {
            GameObject p1Cursor = Instantiate(_cursorPrefab, new Vector2(-8, -2), Quaternion.identity);
            p1Cursor.GetComponent<SpriteRenderer>().color = Color.red;
            p1Cursor.GetComponent<CursorScript>().playerIndex = 0;
            GameObject p2Cursor = Instantiate(_cursorPrefab, new Vector2(-8, -2), Quaternion.identity);
            p2Cursor.GetComponent<SpriteRenderer>().color = Color.blue;
            p2Cursor.GetComponent<CursorScript>().playerIndex = 1;
            statusText.text = "vs";
        }
        if(PlayerPrefs.GetString("Match Type") == "1 Player Local")
        {
            cpuText.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (matchInfo[0] != -1 && matchInfo[1] != -1)
        {
            state = "stage select";
        }
        if (matchInfo[0] != -1 && matchInfo[1] != -1 && matchInfo[2] != -1)
        {
            _readyButton.GetComponent<Image>().color = new Color(0f, 1f, 0f, .4f);
        }
    }

    public void updateMatchInfo(int index, int newVal)
    {
        matchInfo[index] = newVal;
        updateText();
    }
    public int[] getMatchInfo()
    {
        return matchInfo;
    }
    public void updateText()
    {
        statusText.text = characters[matchInfo[0] + 1] + " vs " + characters[matchInfo[1] + 1];
    }
    public void tryToPlay()
    {
        if (matchInfo[0] != -1 && matchInfo[1] != -1 && matchInfo[2] == 6)
        {
            PlayerPrefs.SetInt("Player1Char", matchInfo[0]);
            PlayerPrefs.SetInt("Player2Char", matchInfo[1]);
            PlayerPrefs.SetInt("Stage", matchInfo[2]);
            PlayerPrefs.SetInt("P1 Points", 0);
            PlayerPrefs.SetInt("P2 Points", 0);
            PlayerPrefs.SetString("Gameplay Scene", "TowerScene");
            SceneManager.LoadScene("TowerScene");
        }
        else if (matchInfo[0] != -1 && matchInfo[1] != -1 && matchInfo[2] == 5)
        {
            PlayerPrefs.SetInt("Player1Char", matchInfo[0]);
            PlayerPrefs.SetInt("Player2Char", matchInfo[1]);
            PlayerPrefs.SetInt("Stage", matchInfo[2]);
            PlayerPrefs.SetInt("P1 Points", 0);
            PlayerPrefs.SetInt("P2 Points", 0);
            PlayerPrefs.SetString("Gameplay Scene", "SubwayScene");
            SceneManager.LoadScene("SubwayScene");
        }
        if (matchInfo[0] != -1 && matchInfo[1] != -1 && matchInfo[2] == 4)
        {
            PlayerPrefs.SetInt("Player1Char", matchInfo[0]);
            PlayerPrefs.SetInt("Player2Char", matchInfo[1]);
            PlayerPrefs.SetInt("Stage", matchInfo[2]);
            PlayerPrefs.SetInt("P1 Points", 0);
            PlayerPrefs.SetInt("P2 Points", 0);
            PlayerPrefs.SetString("Gameplay Scene", "HouseScene");
            SceneManager.LoadScene("HouseScene");
        }
    }
}
