using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScript : MonoBehaviour
{
    public GameObject[] buttons;
    int _buttonActive = -1;
    Color inactiveButton;
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
            updateActiveButton(-1);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            updateActiveButton(1);
        }
    }

    void updateActiveButton(int inputNum)
    {
        if(_buttonActive == -1)
        {
            _buttonActive = 0;
        }
        else
        {
            int numButtons = buttons.Length;
            _buttonActive = (_buttonActive + inputNum + numButtons) % numButtons;
        }
        updateButtonColors();
    }

    void updateButtonColors()
    {
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
