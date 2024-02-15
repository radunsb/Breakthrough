using System.Collections;
using System.Collections.Generic;
//using System.Runtime.Remoting.Activation;
using System.Xml.Serialization;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
[RequireComponent(typeof(PlayerInput))]

public class TitleScript : MonoBehaviour
{
    //List of the possible menu options
    public GameObject[] buttons;

    //List of buttons that appear when single-player mode is selected
    public GameObject[] spbuttons;

    //Panel containing all of the buttons for single-player modes
    public GameObject spPanel;

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

    //Public boolean for determining solo or Multiplayer
    public bool isSolo;

    //Number of higher-order menus that are open
    int menuLevel = 0;

    private void Awake()
    {
        controls = new PlayControls();
    }

    private void OnEnable()
    {
        move = controls.Menus.Movement;
        move.performed += navigate;
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
        
    }

    void navigate(InputAction.CallbackContext context)
    {
        Vector2 directions = context.ReadValue<Vector2>();
        if(menuLevel > 0)
        {
            if(directions.x > .5)
            {
                updateSubButton(1, currentSubButtons);
            }
            if(directions.x < -.5)
            {
                updateSubButton(-1, currentSubButtons);
            }
        }
        else
        {
            //Reset the sub-menu counter if there isn't one open
            _subButtonActive = -1;
            if (directions.y > 0.5)
            {
                //Scroll one button up
                updateActiveButton(-1);
            }
            if (directions.y < -0.5)
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
        if(_buttonActive == -1)
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
        if(_subButtonActive == -1)
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
        for(int i = 0; i < currentButtons.Length; i++)
        {
            if(i != active)
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
        //Single-Player Selection
        if(_buttonActive == 0)
        {
            //No Sub-Menu Open
            if(menuLevel == 0)
            {
                spPanel.SetActive(true);
                _subButtonActive = 0;
                currentSubButtons = spbuttons;
                updateButtonColors(currentSubButtons, _subButtonActive);
                menuLevel = 1;
            } 
            //Vs. AI
            else if(_subButtonActive == 0)
            {
                isSolo = true;
                SceneManager.LoadScene("CSScene");
            }
            // 1 V. 1
            else if (_subButtonActive == 1)
            {
                isSolo = false;
                PlayerPrefs.SetString("Match Type", "1v1");
                PlayerPrefs.SetInt("P1 Points", 0);
                PlayerPrefs.SetInt("P2 Points", 0);
                SceneManager.LoadScene("MatchScene");
            }
            else
            {
                //Training Mode
                if(_subButtonActive == 2)
                {
                    isSolo = true;
                    SceneManager.LoadScene("CSScene");
                }
            }
        }

        //Multiplayer Selection
        if(_buttonActive == 1)
        {
            SceneManager.LoadScene("MultiPlayerScene");
        }

        //Options
        if(_buttonActive == 2)
        {
            SceneManager.LoadScene("Options");
        }

        //Exit
        else if(_buttonActive == 3)
        {
            Application.Quit();
        }
    }
    void processBackInput(InputAction.CallbackContext context)
    {
        if (menuLevel > 0)
        {
            spPanel.SetActive(false);
            _subButtonActive = -1;
            menuLevel = 0;
        }
    }
}
