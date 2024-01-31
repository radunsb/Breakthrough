using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScript : MonoBehaviour
{
    //List of the possible menu options
    public GameObject[] buttons;
    //Menu option that is currently being "hovered" over
    int _buttonActive = -1; 
    //Color for a non-hovered button
    Color inactiveButton;
    //Color for a hovered button
    Color activeButton;
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
        updateButtonColors();
    }

    void updateButtonColors()
    {
        //Update each button color only when the selected one is changed (runs from
        //updateActiveButton())
        for(int i = 0; i < buttons.Length; i++)
        {
            if(i != _buttonActive)
            {
                buttons[i].GetComponent<Image>().color = inactiveButton;
            }
            else
            {
                buttons[i].GetComponent<Image>().color = activeButton;
            }
        }
    }
}
