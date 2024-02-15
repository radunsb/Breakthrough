using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using static System.Net.Mime.MediaTypeNames;

[RequireComponent(typeof(PlayerInput))]

public class MultiplayerScript : TitleScript
{
    public override void processBackInput(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene("TitleScene");
    }

    public override void processSelectInput(InputAction.CallbackContext context)
    {
        int _buttonActive = this._buttonActive;
        switch (_buttonActive)
        {
            case 0:
                SceneManager.LoadScene("CSScene");
                break;
            case 1:
                break; //Tourney mode
            case 2:
                break; //Online Multiplayer
            case 3:
                SceneManager.LoadScene("TitleScene");
                break;
            default:
                break;

        }
    }
}


