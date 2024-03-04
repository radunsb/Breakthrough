
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using static System.Net.Mime.MediaTypeNames;

[RequireComponent(typeof(PlayerInput))]

public class CSSManager : TitleScript
{
    
    int[] matchInfo;
    public Text statusText;

    private void Start()
    {
        int numPlayers = PlayerPrefs.GetString("Match Type") == "2 Player Local" ? 2 : 1;
        matchInfo = new int[] { -1, -1, -1 };
        statusText.text = "Player 1 Select Character";
        this._buttonActive = 0;
        this.updateButtonColors(buttons, _buttonActive);
    }
    private void Update()
    {
        if (matchInfo[0] == -1)
        {
            statusText.text = "Player 1 Select Character";
        }
        else if (matchInfo[1] == -1)
        {
            statusText.text = "Player 2 Select Character";
        }
        else if (matchInfo[2] == -1)
        {
            statusText.text = "Select Stage";
        }
        else
        {
            statusText.text = "Ready to Play!";
        }
    }

    protected override void processSelectInput(InputAction.CallbackContext context)
    {
        int _buttonActive = this._buttonActive;
        //Change to between 0 and 3 when other characters are added
        if(_buttonActive <= 1 && (matchInfo[0] == -1 || matchInfo[1] == -1))
        {
            if (matchInfo[0] == -1)
            {
                matchInfo[0] = _buttonActive;
            }
            else
            {
                matchInfo[1] = _buttonActive;
            }
        }
        //Change to between 4 and 6 when other stages added
        else if(_buttonActive == 4 || _buttonActive == 6 && matchInfo[0] != -1 && matchInfo[1] != -1)
        {
            matchInfo[2] = _buttonActive;
        }
        else if(_buttonActive == 7)
        {
            SceneManager.LoadScene("TitleScene");
        }
        else
        {
            if (matchInfo[0] != -1 && matchInfo[1] != -1 && matchInfo[2] == 6)
            {
                PlayerPrefs.SetInt("Player1Char", matchInfo[0]);
                PlayerPrefs.SetInt("Player2Char", matchInfo[1]);
                PlayerPrefs.SetInt("Stage", matchInfo[2]);
                PlayerPrefs.SetInt("P1 Points", 0);
                PlayerPrefs.SetInt("P2 Points", 0);
                SceneManager.LoadScene("MatchScene");
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
    public override void processBackInput(InputAction.CallbackContext context)
    {
        if (matchInfo[2] != -1)
        {
            matchInfo[2] = -1;
        }
        else if (matchInfo[1] != -1)
        {
            matchInfo[1] = -1;
        }
        else if (matchInfo[0] != -1)
        {
            matchInfo[0] = -1;
        }
        else
        {
            SceneManager.LoadScene("TitleScene");
        }
    }

}
