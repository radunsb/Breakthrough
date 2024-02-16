using System.Collections;
using System.Collections.Generic;
//using System.Runtime.Remoting.Activation;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using static System.Net.Mime.MediaTypeNames;

[RequireComponent(typeof(PlayerInput))]

public class OptionsScript : TitleScript
{
    public override void processBackInput(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene("TitleScene");
    }

    protected override void processSelectInput(InputAction.CallbackContext context)
    {
        int _buttonActive = this._buttonActive;
        switch (_buttonActive)
        {
            case 0:
                break; //Volume
            case 1:
                break; //Brightness
            case 2:
                break; //Controls
            case 3:
                SceneManager.LoadScene("TitleScene");
                break;
            default:
                break;

        }
    }
}
