//Script for Non-user controlled characters
//Some code from this will eventually have to be ported over to apply to player characters


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class KnockbackScript : MonoBehaviour
{
    bool _inKnockback;
    //Damage increments every time gameobject is hit
    float _damage;
    Rigidbody2D _rbody;
    //gameObject of the player/other player
    private GameObject opponent;
    PlayerScript _opponentScript;
    public PlayerScript _playerScript;
    public ShieldScript _shieldScript;
    private MatchScript _matchScript;
    int playerIndex;
    public Text dmgText;
    public float movePercent;
    void Start()
    {
        _matchScript = GameObject.FindObjectOfType<MatchScript>();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        playerIndex = _playerScript != null ? _playerScript.playerIndex : -1;
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
        if (!(PlayerPrefs.GetString("Match Type") == "Training" && gameObject.tag.Equals("Player")))
        {
            _opponentScript = opponent.GetComponent<PlayerScript>();
        }       
        movePercent = 1;               
    }

    // Update is called once per frame
    void Update()
    {
        if(_rbody.position.x > 9 || _rbody.position.x < -9 || _rbody.position.y > 5 || _rbody.position.y < -5)
        {
            if (gameObject.tag.Equals("Sandbag"))
            {
                _matchScript.updateCharacterPoints(0);
            }
            else
            {
                if(_playerScript.playerIndex == 0)
                {
                    _matchScript.updateCharacterPoints(1);
                }
                else
                {
                    _matchScript.updateCharacterPoints(0);
                }
            }
        }
        dmgText.text = "Damage: " + _damage;
    }

    private void FixedUpdate()
    {
        if (movePercent > .95)
        {
            movePercent = 1;
        }
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
            if (!_inKnockback || collision.GetComponent<HitboxScript>().multiHit == true)
            {
                HitboxScript hs = collision.gameObject.GetComponent<HitboxScript>();
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
                else
                {
                    _shieldScript.reduceHealth(hs.damage);
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
}
