
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
        if(_buttonActive <= 3 && (matchInfo[0] == -1 || matchInfo[1] == -1))
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
        else if(_buttonActive >= 4 && _buttonActive <= 6 && matchInfo[0] != -1 && matchInfo[1] != -1)
        {
            matchInfo[2] = _buttonActive;
        }
        else if(_buttonActive == 7)
        {
            SceneManager.LoadScene("TitleScene");
        }
        else
        {
            if (matchInfo[0] != -1 && matchInfo[1] != -1 && matchInfo[2] != -1)
            {
                SceneManager.LoadScene("MatchScene");
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
