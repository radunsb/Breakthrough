using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System.Linq;
using System;

public class OnlineShieldScript : ShieldScript
{
    private float _health = 30;
    public PlayerScript _playerScript;
    public OnlinePlayerScript _onlinePlayerScript;
    Queue<int> pingList;
    int currentPing;
    ulong serverId;
    uint tickRate;

    private void FixedUpdate()
    {
        if (shieldActive())
        {
            _health -= 5 / 60f;
        }
        else if (_health < 30)
        {
            _health += 5 / 60f;
        }
        if (_health < -2)
        {
            _health = -2;
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
    }
}
