using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchScript : MonoBehaviour
{
    public List<GameObject> managers;
    public int roundsToWin;
    void Start()
    {
        if(PlayerPrefs.GetString("Match Type") == "1v1")
        {
            Instantiate(managers[0]);
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
