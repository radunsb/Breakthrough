using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(KnockbackScript))]
public class PlayerScript : NetworkBehaviour
{
    public int playerIndex = 0;

    //Character Specific Variables
    public float walkSpeed;
    public float runSpeed;
    public float jump1Height;
    public float jump2Height;
    public GameObject shield;

    //Input controls
    private InputAction normButton;
    private InputAction strongButton;
    protected InputAction inputDirection;
    private InputAction jumpButton;
    private InputAction shieldButton;
    private InputAction pauseButton;
    private InputAction specialButton;
    protected Animator animator;
    private InputActionAsset inputAsset;
    private InputActionMap inGame;

    //Private vars
    public Rigidbody2D _rbody;
    protected int timesJumped;
    protected bool _grounded;
    protected bool _canMove;
    protected bool _isJumping;
    NetworkVariable<bool> _flipX = new NetworkVariable<bool>(false);
    public bool shieldHeld = false;
    protected Vector2 directions;
    protected KnockbackScript knockbackScript;

    //raycast positions
    protected Vector2 bottomLeft;
    protected Vector2 bottomMid;
    protected Vector2 bottomRight;

    protected LayerMask groundLayer;


    protected virtual void Awake()
    {
        inputAsset = this.GetComponent<PlayerInput>().actions;
        //Players should use the "InGame" action map from the PlayControls controller
        //Using FindAction with strings is kind of "blegh", but it's the only way I could
        //get the Input System working with different controllers for different characters
        inGame = inputAsset.FindActionMap("InGame");
    }
    protected virtual void OnEnable()
    {
        //Initialize InputActions

        normButton = inGame.FindAction("Normal_Button");
        normButton.performed += doNormalButton;
        normButton.Enable();

        strongButton = inGame.FindAction("Strong_Button");
        strongButton.performed += doStrongButton;
        strongButton.Enable();

        inputDirection = inGame.FindAction("Input_Direction");
        inputDirection.Enable();

        jumpButton = inGame.FindAction("Jump_Button");
        jumpButton.performed += doJumps;
        jumpButton.Enable();

        specialButton = inGame.FindAction("Special");
        specialButton.performed += doSpecialButton;
        specialButton.Enable();

        shieldButton = inGame.FindAction("Shield_Button");
        shieldButton.performed += (InputAction.CallbackContext context) => { shieldHeld = true; _canMove = false; };
        shieldButton.canceled += (InputAction.CallbackContext context) => { shieldHeld = false; _canMove = true; };
        shieldButton.Enable();

        pauseButton = inGame.FindAction("Pause");
        pauseButton.performed += pause;
        pauseButton.Enable();
    }

    protected virtual void OnDisable()
    {
        //Deactivate InputActions
        normButton.performed -= doNormalButton;
        normButton.Disable();

        strongButton.performed -= doStrongButton;
        strongButton.Disable();

        specialButton.performed -= doSpecialButton;
        specialButton.Disable();

        inputDirection.Disable();

        jumpButton.performed -= doJumps;
        jumpButton.Disable();

        shieldButton.performed += (InputAction.CallbackContext context) => { shieldHeld = true; _canMove = false; };
        shieldButton.canceled += (InputAction.CallbackContext context) => { shieldHeld = false; _canMove = true; };
        shieldButton.Disable();

        pauseButton.performed -= pause;
        pauseButton.Disable();
    }
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        _rbody = GetComponent<Rigidbody2D>();
        shieldHeld = false;
        _grounded = true;
        if (IsServer)
        {
            _flipX.Value = false;
        }

        //Initialize variables for raycasting
        bottomLeft = new Vector2(_rbody.position.x - .5f, _rbody.position.y - 1f);
        bottomMid = new Vector2(_rbody.position.x, _rbody.position.y - 1f);
        bottomRight = new Vector2(_rbody.position.x + .5f, _rbody.position.y - 1f);
        groundLayer = LayerMask.GetMask("Ground");
        timesJumped = 0;

        knockbackScript = GetComponent<KnockbackScript>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("Grounded", _grounded);
        animator.SetBool("InKnockback", knockbackScript._inKnockback);
    }

    private void pause(InputAction.CallbackContext context)
    {
        if (PlayerPrefs.GetString("Match Type") != "2 Player Online")
        {
            SceneManager.LoadScene("TitleScene");
        }
    }
    protected virtual void FixedUpdate()
    {
        //Get the current position of the left joystick/movement keys
        directions = inputDirection.ReadValue<Vector2>();
        //Determines whether forward hold, upward hold, or downward hold (for attacks)
        controlHeldDirection(directions);
        //Controls character's x movement
        Vector2 xMovement = intendedMovement();
        if(_canMove)
        {
            if (knockbackScript.getMovePercent() < 1)
            {
                _rbody.velocity = Vector2.Lerp(_rbody.velocity, xMovement,
                    1/30f);
            }
            else
            {
                _rbody.velocity = xMovement;
            }
        }

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
        animator.ResetTrigger("Normal button");
        animator.ResetTrigger("Strong Button");
    }

    protected virtual void doNormalButton(InputAction.CallbackContext context)
    {
        if (!knockbackScript.getInKnockback())
        {
            animator.SetTrigger("Normal button");
        }
    }

    protected virtual void doStrongButton(InputAction.CallbackContext context)
    {
        if (!knockbackScript.getInKnockback())
        {
            animator.SetTrigger("Strong Button");
        }
    }

    protected virtual void doSpecialButton(InputAction.CallbackContext context)
    {
        if (!knockbackScript.getInKnockback())
        {
            animator.SetTrigger("Special Button");
        }
    }

    protected virtual void doJumps(InputAction.CallbackContext context)
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
    protected virtual void controlHeldDirection(Vector2 dir)
    {
        //default all directions to false
        animator.SetBool("Forward Hold", false);
        animator.SetBool("Upward Hold", false);
        animator.SetBool("Downward Hold", false);
        //Game gives a bit of priority to horizontal holiding. These will probably get adjusted
        //while the game is being refined.
        if (dir.y > 0.6)
        {
            animator.SetBool("Upward Hold", true);            
        }
        else if (dir.y < -0.6)
        {
            animator.SetBool("Downward Hold", true);
        }
        else if(dir.x > 0.4 && !_flipX.Value || dir.x < -0.4 && _flipX.Value)
        {            
            animator.SetBool("Forward Hold", true);
        }      
    }

    //Controls horizontal character movement
    protected virtual Vector2 intendedMovement()
    {
        if (_canMove && !knockbackScript.getInKnockback())
        {
            switch (directions.x)
            {
                //Run right
                case float x when x > .8f:                    
                    _rbody.transform.eulerAngles = new Vector3(0f, 0f, 0);
                    _flipX.Value = false;
                    return new Vector2(runSpeed, _rbody.velocity.y);
                //Walk right
                case float x when (x > 0.4f && x <= .8f):
                    _rbody.transform.eulerAngles = new Vector3(0f, 0f, 0);
                    _flipX.Value = false;
                    return new Vector2(walkSpeed, _rbody.velocity.y);
                //Run left
                case float x when x < -.8f:
                    _rbody.transform.eulerAngles = new Vector3(0f, 180f, 0);
                    _flipX.Value = true;
                    return new Vector2(-runSpeed, _rbody.velocity.y);
                //Walk left
                case float x when (x < -0.4f && x >= -.8f):
                    _rbody.transform.eulerAngles = new Vector3(0f, 180f, 0);
                    _flipX.Value = true;
                    return new Vector2(-walkSpeed, _rbody.velocity.y);
                //No horizontal movement
                default:
                    return new Vector2(0f, _rbody.velocity.y);
            }
        }
        return _rbody.velocity;
    }

    
    protected virtual void singleJump()
    {
        _grounded = false;
        _isJumping = true;
        //Done so that the raycast doesn't immediately make you grounded when you try and jump
        Invoke("resetIsJumping", 0.2f);
        _rbody.velocity = new Vector2(_rbody.velocity.x, jump1Height);
        timesJumped = 1;
        animator.SetTrigger("Jump");
    }

    void resetIsJumping()
    {
        _isJumping = false;
    }

    protected virtual void doubleJump()
    {
        _rbody.velocity = new Vector2(_rbody.velocity.x, jump2Height);
        timesJumped = 2;
        animator.SetTrigger("Jump");
    }
    //Raycast down from the left, middle, and right sides of the player character.
    //Returns true if the raycast hits something belonging to the ground layer.
    protected bool isGrounded()
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
        if(PlayerPrefs.GetString("Match Type") == "2 Player Online" && IsLocalPlayer)
        {
            setAnimationStatesClientRpc("bool", "Ready", true);
        }

    }
    void stopAttackButAllowMovement()
    {
        animator.SetBool("Ready", false);
        if (PlayerPrefs.GetString("Match Type") == "2 Player Online" && IsLocalPlayer)
        {
            setAnimationStatesClientRpc("bool", "Ready", true);
        }
    }
    //Allows the player to attack and move again.
    void canAttack()
    {
        animator.SetBool("Ready", true);
        if (!_canMove) _canMove  = true;
        if (PlayerPrefs.GetString("Match Type") == "2 Player Online" && IsLocalPlayer)
        {
            setAnimationStatesClientRpc("bool", "Ready", true);
        }
    }

    [ClientRpc]
    void setAnimationStatesClientRpc(string type, string name, bool outcome)
    {
        if (type == "trigger")
        {
            animator.SetTrigger(name);
        }
        else if (type == "bool")
        {
            animator.SetBool(name, outcome);
        }
    }

    public bool getFlipX()
    {
        return _flipX.Value;
    }
    public void setFlipX(bool val)
    {
        _flipX.Value = val;
    }

}
