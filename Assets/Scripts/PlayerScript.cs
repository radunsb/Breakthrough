using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Animator))]
public class PlayerScript : MonoBehaviour
{

    public PlayControls controls;
    private InputAction normButton;
    private InputAction inputDirection;
    Animator animator;

    private void Awake()
    {
        controls = new PlayControls();
    }
    private void OnEnable()
    {
        normButton = controls.Just_Attacks.Normal_Button;
        normButton.performed += normalButton;
        normButton.Enable();

        inputDirection = controls.Just_Attacks.Input_Direction;
        inputDirection.performed += controlHeldDirection;
        inputDirection.Enable();
    }

    private void OnDisable()
    {
        normButton.Disable();
        inputDirection.Disable();
    }
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void normalButton(InputAction.CallbackContext context)
    {
        animator.SetTrigger("Normal button");
    }
    void controlHeldDirection(InputAction.CallbackContext context)
    {
        Vector2 directions = context.ReadValue<Vector2>();
        animator.SetBool("Forward Hold", false);
        animator.SetBool("Upward Hold", false);
        animator.SetBool("Downward Hold", false);
        if (directions.y > 0.6)
        {
            animator.SetBool("Upward Hold", true);            
        }
        else if (directions.y < -0.6)
        {
            animator.SetBool("Downward Hold", true);
        }
        else if(directions.x > 0.4)
        {            
            animator.SetBool("Forward Hold", true);
        }
    }
}
