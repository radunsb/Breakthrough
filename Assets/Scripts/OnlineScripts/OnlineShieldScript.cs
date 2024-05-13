using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class OnlineShieldScript : ShieldScript
{
    NetworkVariable<float> _onlineShieldHealth = new NetworkVariable<float>();
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (shieldActive())
        {
            _onlineShieldHealth.Value -= 5 / 60f;
        }
        else if (_onlineShieldHealth.Value < 30)
        {
            _onlineShieldHealth.Value += 5 / 60f;
        }
        if (_onlineShieldHealth.Value < -2)
        {
            _onlineShieldHealth.Value = -2;
        }
        GetComponent<SpriteRenderer>().color = new Color(1 - _onlineShieldHealth.Value / 30f, _onlineShieldHealth.Value / 30f, 0f, shieldActive() ? 0.4f : 0f);
    }
    public bool shieldActive()
    {
        return (_onlineShieldHealth.Value > 0 && _playerScript.shieldHeld);
    }

    public void reduceHealth(float damage)
    {
        _onlineShieldHealth.Value -= damage;
    }

    public void OnlineShieldSync()
    {
        _onlineShieldHealth.Value = _health;
    }
}
