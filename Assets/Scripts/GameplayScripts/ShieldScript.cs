using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class ShieldScript : NetworkBehaviour
{
    protected float _health = 30;
    public PlayerScript _playerScript;

    private void FixedUpdate()
    {
        //If holding shield, gradually decrease health
        if (shieldActive())
        {
            _health -= 5 / 60f;
        }
        //If they're not holding, increase up to full health
        else if(_health < 30)
        {
            _health += 5 / 60f;
        }
        //Don't let it go too negative
        if(_health < -2)
        {
            _health = -2;
        }
        if(_health < .5f)
        {
            _playerScript.shieldHeld = false;
        }
        GetComponent<SpriteRenderer>().color = new Color(1 - _health / 30f, _health / 30f, 0f, shieldActive() ? 0.4f : 0f);
    }
    public bool shieldActive()
    {
        return (_health > 0 && _playerScript.shieldHeld);
    }

    public void reduceHealth(float damage)
    {
        _health -= damage;
        matchHealthServerRpc(_health);
    }

    [ServerRpc]
    public void matchHealthServerRpc(float h)
    {
        _health = h;
    }
}
