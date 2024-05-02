using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OnlineKnockbackScript : KnockbackScript
{
    OnlineMatchScript _oms;
    public NetworkVariable<float> charDamage = new NetworkVariable<float>();
    // Start is called before the first frame update
    protected override void Start()
    {
        charDamage.Value = 0;
        charDamage.OnValueChanged += updateDamageServerRpc;
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

    IEnumerator knockbackBoof(HitboxScript hs, float ld)
    {
        _inKnockback = true;
        for (int i = 0; i < hs.damage; i++)
        {
            movePercent = 0;
            _rbody.velocity = Vector2.zero;
            yield return new WaitForFixedUpdate();
        }
        takeKnockback(hs.velocityMult, ld);

        movePercent = 0;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
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
                    updateDamageServerRpc(charDamage.Value, charDamage.Value + hs.damage);
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
                        updateDamageServerRpc(charDamage.Value, charDamage.Value + hs.damage);
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

    [ServerRpc(RequireOwnership = false)]
    public void updateDamageServerRpc(float previous, float current)
    {
        charDamage.Value = current;
        updateDamageClientRpc();
    }
    [ClientRpc]
    void updateDamageClientRpc()
    {
        _damage = charDamage.Value;
    }

}
