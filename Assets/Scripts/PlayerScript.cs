using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerScript : MonoBehaviour
{
    [SerializeField]
    private int playerIndex = 0;

    //Character Specific Variables
    public float walkSpeed;
    public float runSpeed;
    public float jump1Height;
    public float jump2Height;

    //Input controls
    public PlayControls controls;
    private InputAction normButton;
    private InputAction strongButton;
    private InputAction inputDirection;
    private InputAction jumpButton;
    Animator animator;

    //Private vars
    Rigidbody2D _rbody;
    int timesJumped;
    bool _grounded;
    bool _canMove;
    bool _isJumping;
    public bool _flipX = false;
    Vector2 directions;

    //raycast positions
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
        normButton.performed += doNormalButton;
        normButton.Enable();

        strongButton = controls.InGame.Strong_Button;
        strongButton.performed += doStrongButton;
        strongButton.Enable();

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
        strongButton.Disable();
        inputDirection.Disable();
        jumpButton.Disable();
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        _rbody = GetComponent<Rigidbody2D>();
        _grounded = true;
        _flipX = false;

        //Initialize variables for raycasting
        bottomLeft = new Vector2(_rbody.position.x - .5f, _rbody.position.y - 1f);
        bottomMid = new Vector2(_rbody.position.x, _rbody.position.y - 1f);
        bottomRight = new Vector2(_rbody.position.x + .5f, _rbody.position.y - 1f);
        groundLayer = LayerMask.GetMask("Ground");
        timesJumped = 0;
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("Grounded", _grounded);
    }
    private void FixedUpdate()
    {
        moveCharacter();
        if (isGrounded())
        {
            //if grounded and you didn't just start a jump...
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

    void doNormalButton(InputAction.CallbackContext context)
    {
        animator.SetTrigger("Normal button");
    }

    void doStrongButton(InputAction.CallbackContext context)
    {
        animator.SetTrigger("Strong Button");
    }

    void doJumps(InputAction.CallbackContext context)
    {
        //Determine whether you can jump, and which jump type to do
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
        //default all directions to false
        animator.SetBool("Forward Hold", false);
        animator.SetBool("Upward Hold", false);
        animator.SetBool("Downward Hold", false);
        //Game gives a bit of priority to horizontal holiding. These will probably get adjusted
        //while the game is being refined.
        if (directions.y > 0.6)
        {
            animator.SetBool("Upward Hold", true);            
        }
        else if (directions.y < -0.6)
        {
            animator.SetBool("Downward Hold", true);
        }
        else if(directions.x > 0.4 && !_flipX || directions.x < 0.4 && _flipX)
        {            
            animator.SetBool("Forward Hold", true);
        }      
    }

    //Controls horizontal character movement
    void moveCharacter()
    {
        if (_canMove)
        {
            switch (directions.x)
            {
                //Run right
                case float x when x > .8:
                    _rbody.velocity = new Vector2(runSpeed, _rbody.velocity.y);
                    _rbody.transform.eulerAngles = new Vector3(0f, 0f, 0);
                    _flipX = false;
                    break;
                //Walk right
                case float x when (x > 0.4 && x <= .8):
                    _rbody.velocity = new Vector2(walkSpeed, _rbody.velocity.y);
                    _rbody.transform.eulerAngles = new Vector3(0f, 0f, 0);
                    _flipX = false;
                    break;
                //Run left
                case float x when x < -.8:
                    _rbody.velocity = new Vector2(-runSpeed, _rbody.velocity.y);
                    _rbody.transform.eulerAngles = new Vector3(0f, 180f, 0);
                    _flipX = true;
                    break;
                //Walk left
                case float x when (x < -0.4 && x >= -.8):
                    _rbody.velocity = new Vector2(-walkSpeed, _rbody.velocity.y);
                    _rbody.transform.eulerAngles = new Vector3(0f, 180f, 0);
                    _flipX = true;
                    break;
                //No horizontal movement
                default:
                    _rbody.velocity = new Vector2(0f, _rbody.velocity.y);
                    break;
            }
        }
    }
    void singleJump()
    {
        _grounded = false;
        _isJumping = true;
        //Done so that the raycast doesn't immediately make you grounded when you try and jump
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
        _rbody.velocity = new Vector2(_rbody.velocity.x, jump2Height);
        timesJumped = 2;
    }
    //Raycast down from the left, middle, and right sides of the player character.
    //Returns true if the raycast hits something belonging to the ground layer.
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
    //Used by animator to prevent player from attacking while in a committed state
    void stopAttack()
    {
        animator.SetBool("Ready", false);
        //Grounded attacks should stop the character's movement for their duration
        if (_grounded)
        {
            _canMove = false;
            _rbody.velocity = new Vector2(0f, _rbody.velocity.y);
        }

    }
    //Allows the player to attack and move again.
    void canAttack()
    {
        animator.SetBool("Ready", true);
        if (!_canMove) _canMove  = true;
    }
}
