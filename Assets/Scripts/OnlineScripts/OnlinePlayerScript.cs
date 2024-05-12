using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System.Linq;
using System;

public class OnlinePlayerScript : PlayerScript
{
    Vector2 _oldPos;
    Vector3 _oldRot;
    NetworkVariable<Vector2> _receivedPosn = new NetworkVariable<Vector2>();
    NetworkVariable<Vector3> _receivedRot = new NetworkVariable<Vector3>();
    Queue<int> pingList;
    int currentPing;
    ulong serverId;
    uint tickRate;
    public int preferredStage = -1;
    public int characterType;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if ((IsHost && IsLocalPlayer) || (!IsHost && !IsLocalPlayer))
        {
            playerIndex = 0;
        }
        else
        {
            playerIndex = 1;
        }
        tickRate = NetworkManager.Singleton.NetworkTickSystem.TickRate;
    }
    protected override void Start()
    {
        base.Start();
        pingList = new Queue<int>();
        serverId = NetworkManager.ServerClientId;
        _oldPos = _rbody.position;
        _oldRot = _rbody.transform.eulerAngles;
        if (IsServer)
        {
            _receivedPosn.Value = _rbody.position;
            _receivedRot.Value = _rbody.transform.eulerAngles;
        }

    }
    protected override void FixedUpdate()
    {
        if (IsLocalPlayer)
        {
            ulong ping = NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetCurrentRtt(serverId);
            pingList.Enqueue(Convert.ToInt32(ping.ToString()));
            if (pingList.Count > 5)
            {
                pingList.Dequeue();
            }
            currentPing = pingList.Aggregate(0, (acc, x) => x + acc) / pingList.Count;
            //Get the current position of the left joystick/movement keys
            directions = inputDirection.ReadValue<Vector2>();
            //Determines whether forward hold, upward hold, or downward hold (for attacks)
            controlHeldDirection(directions);
            //Controls character's x movement
            Vector2 xMovement = intendedMovement();
            if (_canMove)
            {
                stepMovement(xMovement);
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
            if (_oldPos != _rbody.position)
            {
                _oldPos = _rbody.position;
                UpdatePosnServerRpc(_rbody.position);
            }
            if (_oldRot != _rbody.transform.eulerAngles)
            {
                _oldRot = _rbody.transform.eulerAngles;
                UpdateRotServerRpc(_rbody.transform.eulerAngles);
            }
            resetTriggerClientRpc("Normal button");
            resetTriggerClientRpc("Strong Button");
        }
        else
        {
            LerpRemote();
        }
    }

    void LerpRemote()
    {
        _rbody.transform.eulerAngles = _receivedRot.Value;
        if (Vector2.Distance(_rbody.position, _receivedPosn.Value) < 0.02)
        {
            return;
        }
        Vector2 velocity = (_receivedPosn.Value - _rbody.position) * tickRate;
        Vector2 target = _receivedPosn.Value + (velocity * currentPing / 2);
        _rbody.position = Vector2.Lerp(_rbody.position, target, 0.5f);
    }

    public void freeze()
    {
        _rbody.velocity = Vector2.zero;
        _rbody.gravityScale = 0f;
    }

    [ServerRpc]
    void UpdatePosnServerRpc(Vector2 pos)
    {
        _receivedPosn.Value = pos;
    }

    [ServerRpc]
    void UpdateRotServerRpc(Vector3 rot)
    {
        _receivedRot.Value = rot;
    }

    private void stepMovement(Vector2 xMovement)
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

    protected override void controlHeldDirection(Vector2 dir)
    {
        directionHandlerServerRpc(dir);
    }

    [ServerRpc]
    void directionHandlerServerRpc(Vector2 dir)
    {
        setAnimationStatesClientRpc("bool", "Forward Hold", false);
        setAnimationStatesClientRpc("bool", "Upward Hold", false);
        setAnimationStatesClientRpc("bool", "Downward Hold", false);
        if (dir.y > 0.6)
        {
            setAnimationStatesClientRpc("bool", "Upward Hold", true);
        }
        else if (dir.y < -0.6)
        {
            setAnimationStatesClientRpc("bool", "Downward Hold", true);
        }
        else if (dir.x > 0.4 && !getFlipX() || dir.x < -0.4 && getFlipX())
        {
            setAnimationStatesClientRpc("bool", "Forward Hold", true);
        }
    }


    [ServerRpc]
    void doNormalServerRpc()
    {
        if (!knockbackScript.getInKnockback())
        {
            setAnimationStatesClientRpc("trigger", "Normal button", true);
        }
    }

    [ServerRpc]
    void doStrongServerRpc()
    {
        if (!knockbackScript.getInKnockback())
        {
            setAnimationStatesClientRpc("trigger", "Strong Button", true);
        }
    }
    [ServerRpc]
    void doSpecialServerRpc()
    {
        if (!knockbackScript.getInKnockback())
        {
            setAnimationStatesClientRpc("trigger", "Special Button", true);
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

    [ClientRpc]
    void resetTriggerClientRpc(string name)
    {
        animator.ResetTrigger(name);
    }

    protected override void singleJump()
    {
        _grounded = false;
        _isJumping = true;
        //Done so that the raycast doesn't immediately make you grounded when you try and jump
        Invoke("resetIsJumping", 0.2f);
        _rbody.velocity = new Vector2(_rbody.velocity.x, jump1Height);
        timesJumped = 1;
        setAnimationStatesClientRpc("trigger", "Jump", true);
    }

    protected override void doubleJump()
    {
        _rbody.velocity = new Vector2(_rbody.velocity.x, jump2Height);
        timesJumped = 2;
        setAnimationStatesClientRpc("trigger", "Jump", true);
    }

    protected override void doJumps(InputAction.CallbackContext context)
    {
        if (IsLocalPlayer)
        {
            base.doJumps(context);
        }
    }
    protected override void doNormalButton(InputAction.CallbackContext context)
    {
        if (IsLocalPlayer)
        {
            doNormalServerRpc();
        }
    }
    protected override void doStrongButton(InputAction.CallbackContext context)
    {
        if (IsLocalPlayer)
        {
            doStrongServerRpc();
        }
    }
    protected override void doSpecialButton(InputAction.CallbackContext context)
    {
        if (IsLocalPlayer)
        {
            doSpecialServerRpc();
        }
    }

    [ServerRpc]
    void setFlipXServerRpc(bool toSet)
    {
        setFlipX(toSet);
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
                    setFlipXServerRpc(false);
                    return new Vector2(runSpeed, _rbody.velocity.y);
                //Walk right
                case float x when (x > 0.4f && x <= .8f):
                    _rbody.transform.eulerAngles = new Vector3(0f, 0f, 0);
                    setFlipXServerRpc(false);
                    return new Vector2(walkSpeed, _rbody.velocity.y);
                //Run left
                case float x when x < -.8f:
                    _rbody.transform.eulerAngles = new Vector3(0f, 180f, 0);
                    setFlipXServerRpc(true);
                    return new Vector2(-runSpeed, _rbody.velocity.y);
                //Walk left
                case float x when (x < -0.4f && x >= -.8f):
                    _rbody.transform.eulerAngles = new Vector3(0f, 180f, 0);
                    setFlipXServerRpc(true);
                    return new Vector2(-walkSpeed, _rbody.velocity.y);
                //No horizontal movement
                default:
                    return new Vector2(0f, _rbody.velocity.y);
            }
        }
        return _rbody.velocity;
    }

}