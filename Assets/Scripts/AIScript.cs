using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIScript : PlayerScript
{

    protected override void Awake()
    {

    }
    protected override void OnEnable()
    {
        
    }
    protected override void OnDisable()
    {

    }
    protected override void Start()
    {
        base.Start();
        directions = Vector2.right;
    }

    protected override void FixedUpdate()
    {
        //Controls character's x movement
        Vector2 xMovement = intendedMovement();
        if (_canMove)
        {
            if (knockbackScript.getMovePercent() < 1)
            {
                _rbody.velocity = Vector2.Lerp(_rbody.velocity, xMovement,
                    1 / 30f);
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
    }

    protected override Vector2 intendedMovement()
    {
        if (_canMove && !knockbackScript.getInKnockback())
        {
            switch (directions.x)
            {
                //Run right
                case float x when x > .8f:
                    _rbody.transform.eulerAngles = new Vector3(0f, 0f, 0);
                    _flipX = false;
                    return new Vector2(runSpeed, _rbody.velocity.y);
                //Walk right
                case float x when (x > 0.4f && x <= .8f):
                    _rbody.transform.eulerAngles = new Vector3(0f, 0f, 0);
                    _flipX = false;
                    return new Vector2(walkSpeed, _rbody.velocity.y);
                //Run left
                case float x when x < -.8f:
                    _rbody.transform.eulerAngles = new Vector3(0f, 180f, 0);
                    _flipX = true;
                    return new Vector2(-runSpeed, _rbody.velocity.y);
                //Walk left
                case float x when (x < -0.4f && x >= -.8f):
                    _rbody.transform.eulerAngles = new Vector3(0f, 180f, 0);
                    _flipX = true;
                    return new Vector2(-walkSpeed, _rbody.velocity.y);
                //No horizontal movement
                default:
                    return new Vector2(0f, _rbody.velocity.y);
            }
        }
        return _rbody.velocity;
    }
}
