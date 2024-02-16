using System.Collections;
using System.Collections.Generic;
//using System.Runtime.Remoting.Activation;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
[RequireComponent(typeof(PlayerInput))]

public class TitleScript : MonoBehaviour
{
    //List of the possible menu options
    public GameObject[] buttons;

    //Menu option that is currently being "hovered" over
    protected int _buttonActive = -1;

    //Color for a non-hovered button
    public Color inactiveButton;

    //Color for a hovered button
    public Color activeButton;

    public PlayControls controls;
    private InputAction move;
    private InputAction confirm;
    private InputAction back;

    //Public boolean for determining solo or Multiplayer
    public bool isSolo;
    private void Start()
    {
        updateButtonColors(buttons, _buttonActive);
    }
    private void Awake()
    {
        controls = new PlayControls();
    }

    protected virtual void OnEnable()
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

    protected void navigate(InputAction.CallbackContext context)
    {
        Vector2 directions = context.ReadValue<Vector2>();
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

    //currentButtons: Current set of buttons (Main, SP, etc.) that is being updated
    //active: ID of the button that is currently active (within currentButtons)
    protected void updateButtonColors(GameObject[] currentButtons, int active)
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

    protected virtual void processSelectInput(InputAction.CallbackContext context)
    {
        //Single-Player Selection
        if(_buttonActive == 0)
        {
            SceneManager.LoadScene("SinglePlayerScene");
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
    public virtual void processBackInput(InputAction.CallbackContext context) { }
}
