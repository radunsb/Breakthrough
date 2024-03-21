using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIScript : PlayerScript
{
    GameObject _opponent;
    PlayerScript _opponentScript;
    float decideNewThingCounter;

    private string status;
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
        _opponent = GetComponent<KnockbackScript>().opponent;
        _opponentScript = _opponent.GetComponent<PlayerScript>();
        status = "Approach";
        base.Start();
    }

    protected override void FixedUpdate()
    {
        decideNewThingCounter++;
        directions = decideJoystickInput(status);
        controlHeldDirection();
        //Controls character's x movement
        Vector2 xMovement = intendedMovement();
        if (_canMove)
        {
            if (decideNewThingCounter > 50)
            {
                decideNewThingCounter = 0;
                status = "Approach";
                if (Mathf.Abs(transform.position.x - _opponent.transform.position.x) < 3
                    && Mathf.Abs(transform.position.y - _opponent.transform.position.y) < 3)
                {
                    if (knockbackScript.getMovePercent() < 1)
                    {
                        status = "Retreat";
                    }
                    else
                    {
                        status = "Attacking";
                        Invoke("tryAttack", 0.1f);
                    }
                }
            }
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
    private Vector2 decideJoystickInput(string mode)
    {
        switch (mode)
        {
            case "Retreat":
                return ((_opponent.transform.position.x < transform.position.x) ? Vector2.right : Vector2.left);
            case "Approach":
                return ((_opponent.transform.position.x < transform.position.x) ? Vector2.left : Vector2.right);
            case "Attack":
                float distX = transform.position.x - _opponent.transform.position.x;
                float distY = transform.position.y - _opponent.transform.position.y;
                if(Mathf.Abs(distX) >= Mathf.Abs(distY))
                {
                    if(distX >= 0)
                    {
                        _rbody.transform.eulerAngles = new Vector3(0f, 180f, 0);
                        _flipX = true;
                        return Vector2.left;
                    }
                    else
                    {
                        _rbody.transform.eulerAngles = new Vector3(0f, 0f, 0);
                        _flipX = false;
                        return Vector2.right;
                    }
                }
                else
                {
                    if(distY >= 0)
                    {
                        return Vector2.down;
                    }
                    else
                    {
                        return Vector2.up;
                    }
                }
            default:
                return Vector2.zero;
            
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
    
    void tryAttack()
    {
        if (!knockbackScript.getInKnockback())
        {
            animator.SetTrigger("Normal button");
        }
    }
}
