using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    //Number of higher-order menus that are open
    int menuLevel = 0;

    void Start()
    {
        inactiveButton = new Color(0.03f, 0f, 0.48f, 0.05f);
        activeButton = new Color(0.03f, 0f, 0.48f, 0.5f);
    }

    void Update()
    {
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

        /** TODO: Update for Controller input
         */

        //Process enter input
        if (Input.GetKeyDown(KeyCode.Z))
        {
            processSelectInput();
        }
        //If there is a sub-menu open...
        if(menuLevel > 0)
        {
            //Back option
            if (Input.GetKeyDown(KeyCode.X))
            {
                processBackInput();
            }
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

    void processSelectInput()
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
            else
            {
                //Training Mode
                if(_subButtonActive == 2)
                {
                    SceneManager.LoadScene("CSScene");
                }
            }
        }
        //Exit
        if(_buttonActive == 3)
        {
            Application.Quit();
        }
    }
    void processBackInput()
    {
        spPanel.SetActive(false);
        _subButtonActive = -1;
        menuLevel = 0;
    }
}
