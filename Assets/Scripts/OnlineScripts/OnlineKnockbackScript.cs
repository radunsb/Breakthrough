using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OnlineKnockbackScript : KnockbackScript
{
    OnlineMatchScript _oms;
    // Start is called before the first frame update
    protected override void Start()
    {
        _oms = GameObject.FindObjectOfType<OnlineMatchScript>();
        StartCoroutine(waitForSecondPlayer());      
    }
    IEnumerator waitForSecondPlayer()
    {
        while (!_oms.checkFor2Players())
        {
            yield return new WaitForSeconds(1);
        }
        base.Start();
    }

    protected override void takeKnockback(float velocityMult, float launchDirection)
    {
        float lm = calcLaunchMultiplier(velocityMult);
        launchDirection = launchDirection * Mathf.Deg2Rad;
        //Initialize a force based on the hitbox's direction and power
        Vector2 force = new Vector2(Mathf.Cos(launchDirection) * lm, Mathf.Sin(launchDirection) * lm);
        takeKnockbackServerRpc(force);
    }

    [ServerRpc]
    void takeKnockbackServerRpc(Vector2 force)
    {
        print("Should be taking knockback");
        _rbody.velocity = Vector2.zero;
        ulong clientId =
        GetComponent<NetworkObject>().OwnerClientId;
        ClientRpcParams param = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        };
        addForceClientRpc(force, param);
    }

    [ClientRpc]
    void addForceClientRpc(Vector2 force, ClientRpcParams param = default)
    {
        _rbody.AddForce(force);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        print("hi");
        if (collision.tag.Equals("Hitbox") && IsLocalPlayer)
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
                    //Character is invulnerable to attacks for a little bit after getting hit
                    //Also subject to change depending on if we implement multi-hit moves
                    takeKnockback(hs.velocityMult, ld);
                    _inKnockback = true;
                    movePercent = 0;
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
                        //Character is invulnerable to attacks for a little bit after getting hit
                        //Also subject to change depending on if we implement multi-hit moves
                        takeKnockback(hs.velocityMult, ld);
                        _inKnockback = true;
                        movePercent = 0;
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
