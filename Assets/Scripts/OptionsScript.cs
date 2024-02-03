using System.Collections;
using System.Collections.Generic;
//using System.Runtime.Remoting.Activation;
using System.Xml.Serialization;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using static System.Net.Mime.MediaTypeNames;

[RequireComponent(typeof(PlayerInput))]

public class OptionsScript : MonoBehaviour
{
    //List of the possible menu options
    public GameObject[] buttons;

    //Menu option that is currently being "hovered" over
    int _buttonActive = -1;

    //Current sub-buttons that are being shown to the player
    GameObject[] currentSubButtons;

    //Button active if a main option is already selected
    int _subButtonActive = -1;

    //Color for a non-hovered button
    Color inactiveButton;

    //Color for a hovered button
    Color activeButton;

    public PlayControls controls;
    private InputAction move;
    private InputAction confirm;
    private InputAction back;

    //Number of higher-order menus that are open
    int menuLevel = 0;

    private void Awake()
    {
        controls = new PlayControls();
    }

    private void OnEnable()
    {
        move = controls.Menus.Movement;
        move.Enable();

        confirm = controls.Menus.Confirm;
        confirm.performed += processSelectInput;
        confirm.Enable();

        back = controls.Menus.Back;
        back.performed += processBackInput;
        back.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
        confirm.Disable();
        back.Disable();
    }
    void Start()
    {
        inactiveButton = new Color(0.03f, 0f, 0.48f, 0.05f);
        activeButton = new Color(0.03f, 0f, 0.48f, 0.5f);
    }

    void Update()
    {
        //If there is a sub-menu open...
        if (menuLevel > 0)
        {
            //Scroll right and left within the sub-menu
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                updateSubButton(1, currentSubButtons);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                updateSubButton(-1, currentSubButtons);
            }
        }
        else
        {
            //Reset the sub-menu counter if there isn't one open
            _subButtonActive = -1;
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                //Scroll one button up
                updateActiveButton(-1);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                //Scroll one button down
                updateActiveButton(1);
            }
        }
    }

    void updateActiveButton(int inputNum)
    {
        //If the user doesn't have an option selected, pushing any navigational key will
        //just select the first button no matter what
        if (_buttonActive == -1)
        {
            _buttonActive = 0;
        }
        //Otherwise, scroll. Variable used so this will still work if we add more buttons
        else
        {
            int numButtons = buttons.Length;
            _buttonActive = (_buttonActive + inputNum + numButtons) % numButtons;
        }
        updateButtonColors(buttons, _buttonActive);
    }

    //inputNum: Indicates the direction the player is scrolling btw the buttons
    //currentButtons: Current set of buttons (Main, SP, etc.) that is being updated
    void updateSubButton(int inputNum, GameObject[] currentButtons)
    {
        if (_subButtonActive == -1)
        {
            _subButtonActive = 0;
        }
        else
        {
            int numButtons = currentButtons.Length;
            _subButtonActive = (_subButtonActive + inputNum + numButtons) % numButtons;
        }
        updateButtonColors(currentButtons, _subButtonActive);
    }

    //currentButtons: Current set of buttons (Main, SP, etc.) that is being updated
    //active: ID of the button that is currently active (within currentButtons)
    void updateButtonColors(GameObject[] currentButtons, int active)
    {
        //Update each button color only when the selected one is changed (runs from
        //updateActiveButton())
        for (int i = 0; i < currentButtons.Length; i++)
        {
            if (i != active)
            {
                currentButtons[i].GetComponent<Image>().color = inactiveButton;
            }
            else
            {
                currentButtons[i].GetComponent<Image>().color = activeButton;
            }
        }
    }

    void processSelectInput(InputAction.CallbackContext context)
    {
        //Adjust Volume
        if (_buttonActive == 0)
        {
            // TO BE DONE
        }

        //Adjust Brightness
        if (_buttonActive == 1)
        {
            // TO BE DONE
        }

        //Adjust controls
        if (_buttonActive == 2)
        {
            // TO BE DONE
        }

        //Return to Title Screen
        else if (_buttonActive == 3)
        {
            SceneManager.LoadScene("TitleScene");
        }
    }
    void processBackInput(InputAction.CallbackContext context)
    {
        if (menuLevel > 0)
        {
            _subButtonActive = -1;
            menuLevel = 0;
        }
    }
}
