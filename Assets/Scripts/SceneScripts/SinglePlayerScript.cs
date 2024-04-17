using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using static System.Net.Mime.MediaTypeNames;

[RequireComponent(typeof(PlayerInput))]

public class SinglePlayerScript : TitleScript
{
    public override void processBackInput(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene("TitleScene");
    }

    protected override void processSelectInput(InputAction.CallbackContext context)
    {
        int _buttonActive = this._buttonActive;
        if(_buttonActive == 0)
        {
            SceneManager.LoadScene("InstructionScene");
        }
        if(_buttonActive == 1)
        {
            SceneManager.LoadScene("CSScene");
            PlayerPrefs.SetString("Match Type", "1 Player Local");
        }
        if(_buttonActive == 2)
        {
            SceneManager.LoadScene("CSScene");
            PlayerPrefs.SetString("Match Type", "Training");
        }
    }
}
