using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIScript : MonoBehaviour
{
    //Character Specific Variables
    public float walkSpeed;
    public float runSpeed;
    public float jump1Height;
    public float jump2Height;
    public List<List<float>> moveRanges = new List<List<float>>();

    //Private vars
    Animator animator;
    Rigidbody2D _rbody;
    int timesJumped;
    bool _grounded;
    bool _canMove;
    bool _isJumping;
    public bool _flipX = false;
    Vector2 directions;

    public GameObject _player;
    PlayerScript _playerScript;

    //raycast positions
    Vector2 bottomLeft;
    Vector2 bottomMid;
    Vector2 bottomRight;

    LayerMask groundLayer;
    LayerMask playerLayer;

    //AI Specific Variables
   
    bool actionable;

    // Start is called before the first frame update
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
        playerLayer = LayerMask.GetMask("Player");
        timesJumped = 0;
        actionable = true;
        _playerScript = _player.GetComponent<PlayerScript>();
        moveRanges.Add(new List<float> { 2, 2, 0, 0 });
        moveRanges.Add(new List<float> { 2, 2, 0, 0 });
        moveRanges.Add(new List<float> { 2, 2, 0, 0 });
        moveRanges.Add(new List<float> { 2, 2, 0, 0 });
        moveRanges.Add(new List<float> { 2, 2, 0, 0 });
        moveRanges.Add(new List<float> { 2, 2, 0, 0 });
        moveRanges.Add(new List<float> { 2, 2, 0, 0 });
        moveRanges.Add(new List<float> { 2, 2, 0, 0 });
        moveRanges.Add(new List<float> { 2, 2, 0, 0 });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
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
        animator.SetBool("Grounded", _grounded);
        if (actionable)
        {
            animator.SetBool("Ready", true);
            print(checkCanAttack());
            switch (checkCanAttack())
            {
                case 0:
                    animator.SetTrigger("Normal button");
                    animator.SetBool("Forward Hold", true);
                    break;
                case 1:
                    animator.SetTrigger("Normal button");
                    animator.SetBool("Upward Hold", true);
                    break;
                case 2:
                    animator.SetTrigger("Normal button");
                    animator.SetBool("Downward Hold", true);
                    break;
                case 3:
                    animator.SetTrigger("Strong Button");
                    animator.SetBool("Forward Hold", true);
                    break;
                case 4:
                    animator.SetTrigger("Strong Button");
                    animator.SetBool("Upward Hold", true);
                    break;
                case 5:
                    animator.SetTrigger("Strong Button");
                    animator.SetBool("Downward Hold", true);
                    break;
                default:
                    animator.ResetTrigger("Normal button");
                    animator.ResetTrigger("Strong Button");
                    break;
            }
        }
        else
        {
            animator.SetBool("Ready", false);
        }
    }

    int checkCanAttack()
    {      
        if (_grounded)
        {
            for(int i = 0; i <= 5; i++)
            {
                Vector2 topLeft = new Vector2(_rbody.position.x + moveRanges[i][2], _rbody.position.y + moveRanges[i][1]);
                Vector2 bottomLeft = new Vector2(_rbody.position.x + moveRanges[i][2], _rbody.position.y + moveRanges[i][3]);
                float xRange = moveRanges[i][0] + moveRanges[i][2];
                RaycastHit2D hitPlayerTop = Physics2D.Raycast(topLeft, Vector2.right, xRange, playerLayer);
                RaycastHit2D hitPlayerBottom = Physics2D.Raycast(bottomLeft, Vector2.right, xRange, playerLayer);
                if(hitPlayerTop.collider != null || hitPlayerBottom.collider != null)
                {
                    return i;
                }
            }
        }
        return -1;
    }
    void stopAttack()
    {
        //animator.SetBool("Ready", false);
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
        if (!_canMove) _canMove = true;
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
