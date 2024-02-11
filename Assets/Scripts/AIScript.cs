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
                    break;
            }
        }
    }

    int checkCanAttack()
    {
        float xDistance = _player.transform.position.x - transform.position.x;
        float yDistance = _player.transform.position.y - transform.position.y;       
        if (_grounded)
        {
            for(int i = 0; i <= 5; i++)
            {
                RaycastHit2D hitPlayer = Physics2D.Raycast(_rbody.position, Vector2.right,
                    moveRanges[i][0], playerLayer);
                if(hitPlayer.collider != null)
                {
                    return i;
                }
            }
        }
        return -1;
    }
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
