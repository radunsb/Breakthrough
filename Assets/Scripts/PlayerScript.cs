using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerScript : MonoBehaviour
{
    public float walkSpeed;
    public float runSpeed;
    public float jump1Height;
    public float jump2Height;

    public PlayControls controls;
    private InputAction normButton;
    private InputAction inputDirection;
    private InputAction jumpButton;
    Animator animator;

    Rigidbody2D _rbody;
    int timesJumped;
    bool _grounded;
    bool _isJumping;
    public bool _flipX = false;
    Vector2 directions;

    Vector2 bottomLeft;
    Vector2 bottomMid;
    Vector2 bottomRight;

    LayerMask groundLayer;

    private void Awake()
    {
        controls = new PlayControls();
        
    }
    private void OnEnable()
    {
        normButton = controls.InGame.Normal_Button;
        normButton.performed += normalButton;
        normButton.Enable();

        inputDirection = controls.InGame.Input_Direction;
        inputDirection.performed += controlHeldDirection;
        inputDirection.Enable();

        jumpButton = controls.InGame.Jump_Button;
        jumpButton.performed += doJumps;
        jumpButton.Enable();
    }

    private void OnDisable()
    {
        normButton.Disable();
        inputDirection.Disable();
        jumpButton.Disable();
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        _rbody = GetComponent<Rigidbody2D>();
        _grounded = true;
        _flipX = false;
        bottomLeft = new Vector2(_rbody.position.x - .5f, _rbody.position.y - 1f);
        bottomMid = new Vector2(_rbody.position.x, _rbody.position.y - 1f);
        bottomRight = new Vector2(_rbody.position.x + .5f, _rbody.position.y - 1f);
        groundLayer = LayerMask.GetMask("Ground");
        timesJumped = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        moveCharacter();
        if (isGrounded())
        {
            print("grounded");
            if (!_isJumping)
            {
                _grounded = true;
                timesJumped = 0;
            }
        }
        else
        {
            _grounded = false;
        }
    }

    void normalButton(InputAction.CallbackContext context)
    {
        animator.SetTrigger("Normal button");
    }

    void doJumps(InputAction.CallbackContext context)
    {
        switch (timesJumped)
        {
            case 0:
                singleJump();
                break;
            case 1:
                doubleJump();
                break;
            default:
                break;
        }
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
        if (_grounded)
        {
            switch (directions.x)
            {
                case float x when x > .8:
                    _rbody.velocity = new Vector2(runSpeed, _rbody.velocity.y);
                    _rbody.transform.eulerAngles = new Vector3(0f, 0f, 0);
                    _flipX = false;
                    break;
                case float x when (x > 0.3 && x <= .8):
                    _rbody.velocity = new Vector2(walkSpeed, _rbody.velocity.y);
                    _rbody.transform.eulerAngles = new Vector3(0f, 0f, 0);
                    _flipX = false;
                    break;
                case float x when x < -.8:
                    _rbody.velocity = new Vector2(-runSpeed, _rbody.velocity.y);
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
    void singleJump()
    {
        print("sj");
        _grounded = false;
        _isJumping = true;
        Invoke("resetIsJumping", 0.2f);
        _rbody.velocity = new Vector2(_rbody.velocity.x, jump1Height);
        timesJumped = 1;
    }

    void resetIsJumping()
    {
        _isJumping = false;
    }

    void doubleJump()
    {
        print("dj");
        _rbody.velocity = new Vector2(_rbody.velocity.x, jump2Height);
        timesJumped = 2;
    }
    bool isGrounded()
    {
        bottomLeft = new Vector2(_rbody.position.x - .5f, _rbody.position.y - 1f);
        bottomMid = new Vector2(_rbody.position.x, _rbody.position.y - 1f);
        bottomRight = new Vector2(_rbody.position.x + .5f, _rbody.position.y - 1f);
        RaycastHit2D hitLeft = Physics2D.Raycast(bottomLeft, Vector2.down, 0.5f, groundLayer);
        RaycastHit2D hitCenter = Physics2D.Raycast(bottomMid, Vector2.down, 0.5f, groundLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(bottomRight, Vector2.down, 0.5f, groundLayer);
        return (hitLeft.collider != null || hitCenter.collider != null || hitRight.collider != null);
    }
}
