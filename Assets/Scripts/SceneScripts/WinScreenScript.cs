using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class WinScreenScript : TitleScript
{
    public Text winnerText;
    protected override void Start()
    {
        base.Start();
        if(PlayerPrefs.GetInt("P1 Points") > PlayerPrefs.GetInt("P2 Points"))
        {
            winnerText.text = "Player One Wins!";
        }
        else
        {
            winnerText.text = "Player Two Wins!";
        }
    }

    private void Update()
    {
        
    }
    protected override void navigate(InputAction.CallbackContext context)
    {
        Vector2 directions = context.ReadValue<Vector2>();
        if (directions.x > 0.5)
        {
            //Scroll one button right
            updateActiveButton(1);
        }
        if (directions.x < -0.5)
        {
            //Scroll one button left
            updateActiveButton(-1);
        }
    }

    protected override void processSelectInput(InputAction.CallbackContext context)
    {
        switch (_buttonActive)
        {
            case 0:
                SceneManager.LoadScene("TitleScene");
                break;
            case 1:
                SceneManager.LoadScene("CSScene");
                break;
            case 2:
                PlayerPrefs.SetInt("P1 Points", 0);
                PlayerPrefs.SetInt("P2 Points", 0);
                SceneManager.LoadScene(PlayerPrefs.GetString("Gameplay Scene"));
                break;
            default:
                break;
        }
    }

    public override void processBackInput(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene("TitleScene");
    }
}
