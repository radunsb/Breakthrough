using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkSetupScript : TitleScript
{

    public InputField _if;

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
        //Single-Player Selection
        if (_buttonActive == 0)
        {
            _if.Select();
        }

        //Multiplayer Selection
        if (_buttonActive == 1)
        {
            SceneManager.LoadScene("TitleScene");
        }

        //Options
        if (_buttonActive == 2)
        {
            PlayerPrefs.SetString("targetIP", _if.text);
            SceneManager.LoadScene("CSScene");
        }

    }
}
