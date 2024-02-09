//Script for Non-user controlled characters
//Some code from this will eventually have to be ported over to apply to player characters


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyScript : MonoBehaviour
{
    bool _inKnockback;
    //Damage increments every time gameobject is hit
    float _damage;
    Rigidbody2D _rbody;
    //gameObject of the player/other player
    public GameObject opponent;
    PlayerScript _opponentScript;
    public Text dmgText;
    void Start()
    {
        _inKnockback = false;
        _damage = 0;
        _rbody = GetComponent<Rigidbody2D>();
        _opponentScript = opponent.GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        dmgText.text = "Damage: " + _damage;
    }

    //Knockback as a function of the hitbox's power and the character's damage
    //(the function itself is subject to change)
    float calcLaunchMultiplier(float velocityMult)
    {
        return velocityMult * (_damage / 50 + 1) * 200;
    }

    void takeKnockback(float velocityMult, float launchDirection)
    {
        float lm = calcLaunchMultiplier(velocityMult);
        launchDirection = launchDirection * Mathf.Deg2Rad;
        //Initialize a force based on the hitbox's direction and power
        Vector2 force = new Vector2(Mathf.Cos(launchDirection) * lm, Mathf.Sin(launchDirection) * lm);
        //Set the current velocity to zero so moves do knockback consistently
        _rbody.velocity = Vector2.zero;
        _rbody.AddForce(force);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Hitbox"))
        {
            if (!_inKnockback)
            {
                HitboxScript hs = collision.gameObject.GetComponent<HitboxScript>();
                float ld = hs.launchDirection;
                _damage += hs.damage;
                //for now, just flip the launch direction if attacker is facing left
                //might have to be made more complex depending on the kinds of moves we add
                if (_opponentScript._flipX)
                {
                    ld = 180 - ld;
                }
                //Character is invulnerable to attacks for a little bit after getting hit
                //Also subject to change depending on if we implement multi-hit moves
                takeKnockback(hs.velocityMult, ld);
                _inKnockback = true;
                Invoke("allowKnockback", 0.2f);
            }
        }
    }

    void allowKnockback()
    {
        _inKnockback = false;
    }
}
