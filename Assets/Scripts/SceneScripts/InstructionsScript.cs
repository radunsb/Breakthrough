using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class InstructionsScript : MonoBehaviour
{
    public PlayControls controls;
    private InputAction move;
    private InputAction back;
    public GameObject[] pages;
    int curActive;
    private void Awake()
    {
        controls = new PlayControls();
    }

    private void OnEnable()
    {
        move = controls.Menus.Movement;
        move.performed += navigate;
        move.Enable();
        back = controls.Menus.Back;
        back.performed += processBackInput;
        back.Enable();
    }
    private void OnDisable()
    {
        move.performed -= navigate;
        move.Disable();
        back.performed -= processBackInput;
        back.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        pages[0].SetActive(true);
        curActive = 0;
    }
    void navigate(InputAction.CallbackContext context)
    {
        Vector2 directions = context.ReadValue<Vector2>();
        if(directions.x > 0.5)
        {
            pageForward();
        }
        else if(directions.x < 0.5)
        {
            pageBackward();
        }
    }

    void pageForward()
    {
        if (curActive < pages.Length - 1)
        {
            pages[curActive].SetActive(false);
            curActive = (curActive + 1);
            pages[curActive].SetActive(true);
        }

    }
    void pageBackward()
    {
        if (curActive > 0)
        {
            pages[curActive].SetActive(false);
            curActive = (curActive - 1) % pages.Length;
            pages[curActive].SetActive(true);
        }
    }

    void processBackInput(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene("SinglePlayerScene");
    }
}
