//TODO
//Script for the controller remap options screen

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ControlRemapScript : TitleScript
{
    protected override void processSelectInput(InputAction.CallbackContext context)
    {

    }
    public override void processBackInput(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene("Options");
    }
}
