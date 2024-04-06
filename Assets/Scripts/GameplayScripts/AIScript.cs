using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIScript : PlayerScript
{
    GameObject _opponent;
    PlayerScript _opponentScript;
    float decideNewThingCounter = -20;

    public string status;
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
        status = "Stationary";
        base.Start();
    }

    protected override void FixedUpdate()
    {
        decideNewThingCounter++;
        directions = decideJoystickInput(status);
        controlHeldDirection(directions);
        //Controls character's x movement
        Vector2 xMovement = intendedMovement();
        if (_canMove)
        {
            if(shieldHeld == true)
            {

            }
            if (decideNewThingCounter > 30)
            {
                decideNewThingCounter = 0;
                shieldHeld = false;
                status = "Approaching";

                if (Mathf.Abs(transform.position.x - _opponent.transform.position.x) < 1.5
                    && Mathf.Abs(transform.position.y - _opponent.transform.position.y) < 2.5)
                {
                    float randNum = Random.Range(0f, 1f);
                    if (randNum >= 0.5f)
                    {
                        status = "Retreating";
                    }
                    else
                    {
                        status = "Attacking";
                        directions = decideJoystickInput(status);
                        Invoke("tryAttack", 0.1f);
                    }
                    
                }
                else if (Mathf.Abs(transform.position.x - _opponent.transform.position.x) < 3
                    && Mathf.Abs(transform.position.y - _opponent.transform.position.y) < 3)
                {
                    float randNum = Random.Range(0f, 1f);
                    if (randNum >= .4)
                    {
                        if (knockbackScript.getMovePercent() < 1)
                        {
                            status = "Retreating";
                        }
                        else
                        {
                            status = "Attacking";
                            directions = decideJoystickInput(status);
                            Invoke("tryAttack", 0.1f);
                        }
                    }
                    else if (randNum >= .2 && randNum < .4)
                    {
                        status = "Retreating";
                        shieldHeld = true;
                    }
                    else if (randNum >= .1 && randNum < .2)
                    {
                        status = "Approaching";
                        shieldHeld = true;
                    }
                    else
                    {
                        status = "Stationary";
                        shieldHeld = true;
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
                decideIfJump();
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
            case "Retreating":
                return ((_opponent.transform.position.x < transform.position.x) ? Vector2.right : Vector2.left);
            case "Approaching":
                return ((_opponent.transform.position.x < transform.position.x) ? Vector2.left : Vector2.right);
            case "Attacking":
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
            if (isGrounded())
            {
                float randNum = Random.Range(0f, 1f);
                if (randNum >= .7f)
                {
                    animator.SetTrigger("Strong Button");
                }
                else
                {
                    animator.SetTrigger("Normal button");
                }
            }
            else
            {
                animator.SetTrigger("Normal button");
            }
        }
    }

    void decideIfJump()
    {
        if(status == "Approaching" || status == "Attacking")
        {
            float randNum = Random.Range(0f, 1f);
            if(randNum >= 0.99f)
            {
                singleJump();
            }
        }
    }
}
