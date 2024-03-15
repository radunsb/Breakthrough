//Script to the control the player knockback and movement following hit

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class KnockbackScript : MonoBehaviour
{
    //Active for 0.2 seconds following a hit
    bool _inKnockback;
    //Damage increments every time gameobject is hit
    float _damage;
    Rigidbody2D _rbody;
    //gameObject of the player/other player
    private GameObject opponent;
    PlayerScript _opponentScript;
    public PlayerScript _playerScript;
    public ShieldScript _shieldScript;
    int playerIndex;
    public Text dmgText;
    //Percentage that the player's input overrides existing velocity
    //Set to 0 on hit and gradually increases back to 1
    public float movePercent;
    void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        //player index = playerindex if NOT CPU, otherwise make -1
        playerIndex = _playerScript != null ? _playerScript.playerIndex : -1;
        //Set the opponent for each entity in game
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PlayerScript>().playerIndex != playerIndex)
            {
                opponent = player;
            }
        }
        _inKnockback = false;
        _damage = 0;
        _rbody = GetComponent<Rigidbody2D>();
        //If we have two players, set the opponentScript to the other player's playerScript
        if (!(PlayerPrefs.GetString("Match Type") == "Training" && gameObject.tag.Equals("Player")))
        {
            _opponentScript = opponent.GetComponent<PlayerScript>();
        }
        movePercent = 1;
    }

    // Update is called once per frame
    void Update()
    {

        dmgText.text = "Damage: " + _damage;
    }

    private void FixedUpdate()
    {
        //Makes sure we don't get floating point issues
        if (movePercent > .95)
        {
            movePercent = 1;
        }
        //Should take around .6 seconds to get to full movePercent
        if (movePercent < 1)
        {
            movePercent += (1 / 30f);
        }
    }

    //Knockback as a function of the hitbox's power and the character's damage
    //(the function itself is subject to change)
    float calcLaunchMultiplier(float velocityMult)
    {
        return velocityMult * (_damage / 100 + 1) * 200;
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
                //If there is not an active shield, hit and do knockback
                if (gameObject.tag.Equals("Sandbag") || !_shieldScript.shieldActive())
                {
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
                //if there is an active shield, do damage to the shield
                else
                {
                    _shieldScript.reduceHealth(hs.damage);
                    //If that last hit broke shield, do knockback to player
                    if (!_shieldScript.shieldActive())
                    {
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
                    else
                    {
                        _inKnockback = true;
                        Invoke("allowKnockback", 0.2f);
                    }
                }
            }
        }
    }

    void allowKnockback()
    {
        movePercent = 0f;
        _inKnockback = false;
    }

    public float getMovePercent()
    {
        return movePercent;
    }

    public bool getInKnockback()
    {
        return _inKnockback;
    }

    public void setOpponent(GameObject opponent)
    {
        this.opponent = opponent;
    }

    public void setDamage(float damage)
    {
        _damage = damage;
    }
}
