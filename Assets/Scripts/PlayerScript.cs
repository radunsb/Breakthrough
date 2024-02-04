using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerScript : MonoBehaviour
{
    public float walkSpeed;
    public float runSpeed;

    public PlayControls controls;
    private InputAction normButton;
    private InputAction inputDirection;
    Animator animator;

    Rigidbody2D _rbody;
    bool _grounded;
    public bool _flipX = false;
    Vector2 directions;

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
        _rbody = GetComponent<Rigidbody2D>();
        _grounded = true;
        _flipX = false;
    }

    // Update is called once per frame
    void Update()
    {
        moveCharacter();
    }
    void normalButton(InputAction.CallbackContext context)
    {
        animator.SetTrigger("Normal button");
    }
    void controlHeldDirection(InputAction.CallbackContext context)
    {
        directions = context.ReadValue<Vector2>();
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
        else if(directions.x > 0.3 && !_flipX || directions.x < 0.3 && _flipX)
        {            
            animator.SetBool("Forward Hold", true);
        }      
    }

    void moveCharacter()
    {
        if (_grounded) {
            switch (directions.x)
            {
                case float x when x > .8:
                    _rbody.velocity = new Vector2(runSpeed, 0f);
                    _rbody.transform.eulerAngles = new Vector3(0f, 0f, 0);
                    _flipX = false;
                    break;
                case float x when (x > 0.3 && x <= .8):
                    _rbody.velocity = new Vector2(walkSpeed, 0f);
                    _rbody.transform.eulerAngles = new Vector3(0f, 0f, 0);
                    _flipX = false;
                    break;
                case float x when x < -.8:
                    _rbody.velocity = new Vector2(-runSpeed, 0f);
                    _rbody.transform.eulerAngles = new Vector3(0f, 180f, 0);
                    _flipX = true;
                    break;
                case float x when (x < -0.3 && x >= -.8):
                    _rbody.velocity = new Vector2(-walkSpeed, 0f);
                    _rbody.transform.eulerAngles = new Vector3(0f, 180f, 0);
                    _flipX = true;
                    break;
                default:
                    _rbody.velocity = new Vector2(0f, 0f);
                    break;
            }
        }
    }
}
