using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class CursorScript : MonoBehaviour
{
    private InputActionAsset inputAsset;
    private InputActionMap characterSelect;
    private InputAction move;
    private InputAction confirm;
    private InputAction back;

    Rigidbody2D _rbody;

    public int activeButton = -1;
    public int playerIndex;
    NewCSScript script;
    private void Awake()
    {
        inputAsset = this.GetComponent<PlayerInput>().actions;
        characterSelect = inputAsset.FindActionMap("CharacterSelect");
    }
    private void OnEnable()
    {
        move = characterSelect.FindAction("Move");
        move.Enable();
        confirm = characterSelect.FindAction("Confirm");
        confirm.performed += onConfirm;
        confirm.Enable();
        back = characterSelect.FindAction("Back");
    }
    private void OnDisable()
    {
        move.Disable();
        confirm.performed -= onConfirm;
        confirm.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        script= GameObject.FindObjectOfType<NewCSScript>();
        _rbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        moveCursor();
    }
    void moveCursor()
    {
        //handles 2-dimensional cursor movement with analog stick/keyboard
        _rbody.velocity = move.ReadValue<Vector2>() * 8;
    }
    void onConfirm(InputAction.CallbackContext context)
    {
        //Back button
        if(activeButton == 7)
        {
            SceneManager.LoadScene("TitleScene");
        }
        //allow players to choose character during player select stage
        else if(script.state == "player select" && activeButton >= 0 && activeButton <= 3)
        {
            script.updateMatchInfo(playerIndex, activeButton);
            //update the preview text at the bottom of the screen
            script.updateText();
        }
        //allow players to choose stage during stage select stage
        else if(script.state == "stage select" && activeButton >= 4 && activeButton <= 6)
        {
            script.updateMatchInfo(2, activeButton);
        }
        //try to start the game
        else
        {
            script.tryToPlay();
        }
    }
}
