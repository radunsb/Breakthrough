//Script to the control the player knockback and movement following hit

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class KnockbackScript : NetworkBehaviour
{
    protected AudioSource _as;
    public AudioClip hitsound1;
    public AudioClip hitsound2;
    public AudioClip shieldHitsound;
    //Active for 0.2 seconds following a hit
    public bool _inKnockback;
    //Damage increments every time gameobject is hit
    protected float _damage;
    protected Rigidbody2D _rbody;
    //gameObject of the player/other player
    public GameObject opponent;
    public PlayerScript _opponentScript;
    public PlayerScript _playerScript;
    public ShieldScript _shieldScript;
    int playerIndex;
    Text dmgText;
    //Percentage that the player's input overrides existing velocity
    //Set to 0 on hit and gradually increases back to 1
    public float movePercent;
    protected virtual void Start()
    {
        _as = GetComponent<AudioSource>();
        _as.volume = (PlayerPrefs.HasKey("Volume")) ? PlayerPrefs.GetFloat("Volume") : 1.0f;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        //player index = playerindex if NOT sandbag, otherwise make -1
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
        if(playerIndex == 0)
        {
            dmgText = GameObject.FindGameObjectWithTag("DMG0").gameObject.GetComponent<Text>();
            dmgText.color = Color.cyan;
        }
        else
        {
            dmgText = GameObject.FindGameObjectWithTag("DMG1").gameObject.GetComponent<Text>();
            dmgText.color = new Color(1f, 0.4f, 0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (dmgText != null)
        {
            dmgText.text = _damage + "%";
        }
    }

    private void FixedUpdate()
    {
        //Makes sure we don't get floating point issues
        if (movePercent > .98)
        {
            movePercent = 1;
        }
        //Should take around 1 second to get to full movePercent
        if (movePercent < 1)
        {
            movePercent += (1 / 50f);
        }
        //Double checks that inKnockback is false after 0.2 seconds (avoids weird buggy thing)
        if(movePercent > 10 / 50f)
        {
            _inKnockback = false;
        }
    }

    //Knockback as a function of the hitbox's power and the character's damage
    //(the function itself is subject to change)
    protected float calcLaunchMultiplier(float velocityMult)
    {
        return velocityMult * (_damage / 100 + 1) * 200;
    }

    protected virtual void takeKnockback(float velocityMult, float launchDirection)
    {
        movePercent = 0;
        float lm = calcLaunchMultiplier(velocityMult);
        launchDirection = launchDirection * Mathf.Deg2Rad;
        //Initialize a force based on the hitbox's direction and power
        Vector2 force = new Vector2(Mathf.Cos(launchDirection) * lm, Mathf.Sin(launchDirection) * lm);
        //Set the current velocity to zero so moves do knockback consistently
        _rbody.velocity = Vector2.zero;
        _rbody.AddForce(force);
    }

    IEnumerator knockbackBoof(HitboxScript hs,float ld)
    {
        _inKnockback = true;
        for (int i = 0; i < hs.damage * 2; i++)
        {
            movePercent = 0;
            _rbody.velocity = Vector2.zero;
            _rbody.gravityScale = 0f;
            _opponentScript._rbody.velocity = Vector2.zero;
            _opponentScript._rbody.gravityScale = 0f;
            yield return new WaitForFixedUpdate();
        }
        _rbody.gravityScale = 1.8f;
        _opponentScript._rbody.gravityScale = 1.8f;
        takeKnockback(hs.velocityMult, ld);
        
        movePercent = 0;
    }


    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Hitbox"))
        {
            if (!_inKnockback)
            {
                HitboxScript hs = collision.gameObject.GetComponent<HitboxScript>();
                //If there is not an active shield, hit and do knockback
                if (gameObject.tag.Equals("Sandbag") || !_shieldScript.shieldActive())
                {
                    _as.PlayOneShot(Random.Range(0f, 1f) < 0.5 ? hitsound1 : hitsound2);
                    float ld = hs.launchDirection;
                    _damage += hs.damage;
                    //for now, just flip the launch direction if attacker is facing left
                    //might have to be made more complex depending on the kinds of moves we add
                    if (_opponentScript.getFlipX())
                    {
                        ld = 180 - ld;
                    }
                    StartCoroutine(knockbackBoof(hs, ld));

                }
                //if there is an active shield, do damage to the shield
                else
                {
                    _shieldScript.reduceHealth(hs.damage);
                    //If that last hit broke shield, do knockback to player
                    if (!_shieldScript.shieldActive())
                    {
                        _as.PlayOneShot(Random.Range(0f, 1f) < 0.5 ? hitsound1 : hitsound2);
                        float ld = hs.launchDirection;
                        _damage += hs.damage;
                        //for now, just flip the launch direction if attacker is facing left
                        //might have to be made more complex depending on the kinds of moves we add
                        if (_opponentScript.getFlipX())
                        {
                            ld = 180 - ld;
                        }
                        StartCoroutine(knockbackBoof(hs, ld));
                    }
                    else
                    {
                        _as.PlayOneShot(shieldHitsound);
                        _inKnockback = true;
                        movePercent = 0;
                    }
                }
            }
        }
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

    public float getDamage()
    {
        return _damage;
    }
}
