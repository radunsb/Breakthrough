using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class OnlinePlayerScript : PlayerScript
{
    Vector2 _oldPos;
    NetworkVariable<Vector2> _receivedPosn = new NetworkVariable<Vector2>();
    protected override void Start()
    {
        base.Start();
        _oldPos = _rbody.position;
        if (IsServer)
        {
            _receivedPosn.Value = _rbody.position;
        }
    }
    protected override void FixedUpdate()
    {
        if (IsLocalPlayer) {
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
            animator.ResetTrigger("Normal button");
            animator.ResetTrigger("Strong Button");
        }
        else
        {
            LerpRemote();
        }
    }

    void LerpRemote()
    {
        float velo = (Vector2.Distance(_receivedPosn.Value, _rbody.position) * NetworkManager.Singleton.NetworkTickSystem.TickRate);
        if (Vector2.Distance(_rbody.position, _receivedPosn.Value) < 0.1)
        {
            return;
        }
        Vector2 target = _receivedPosn.Value;
        _rbody.position = Vector2.Lerp(_rbody.position, _receivedPosn.Value, 0.4f);
    }

    [ServerRpc]
    void UpdatePosnServerRpc(Vector2 pos)
    {
        _receivedPosn.Value = pos;
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
   
}
