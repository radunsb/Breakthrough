using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
[RequireComponent(typeof(AudioSource))]

public class BreakScript : NetworkBehaviour
{
    
    float _health = 100;
    //List of healthy and damaged sprites
    public Sprite[] sprites;
    MatchScript _ms;
    bool[] hasPlayedSFX = { false, false, false };
    bool isOnline;
    NetworkVariable<float> onlineHealth = new NetworkVariable<float>(100);
    KnockbackScript ks;
    SpriteRenderer _sr;

    private void Start()
    {      
        isOnline = PlayerPrefs.GetString("Match Type") == "2 Player Online";
        _ms = GameObject.FindObjectOfType<MatchScript>();
        _sr = GetComponent<SpriteRenderer>();
        _sr.material.color = Color.white;
        onlineHealth.OnValueChanged += updateDamageServerRpc;
        StartCoroutine(wallPulsing(5, Color.green));
    }

    IEnumerator wallPulsing(float cycleTime, Color target)
    {
        for (int i = 0; i < 30; i++)
        {
            _sr.material.color = Color.Lerp(_sr.material.color, target, 0.1f);
            yield return new WaitForFixedUpdate();
        }
        for (int i = 0; i < 30; i++)
        {
            _sr.material.color = Color.Lerp(_sr.material.color, Color.white, 0.1f);
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(cycleTime);
        StartCoroutine(wallPulsing(cycleTime, target));
    }


    [ClientRpc]
    void doBreakingClientRpc(int state)
    {
        switch (state)
        {
            case 0:
                GetComponent<SpriteRenderer>().sprite = sprites[1];
                break;
            case 1:
                GetComponent<SpriteRenderer>().sprite = sprites[2];
                break;
            case 2:
                gameObject.SetActive(false);
                break;
            default: break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player" || collision.gameObject.tag == "Sandbag")
        {
            if (!isOnline)
            {
                Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
                ks = collision.gameObject.GetComponent<KnockbackScript>();
                //Makes sure player impacting is actively being knocked back
                //Allows us to avoid having floor take damage from jumping, etc.
                if (ks.movePercent < 1)
                {
                    _health -= (ks.getDamage() / 7f * (1.2f - ks.movePercent));
                }
                else if (gameObject.tag.Equals("Ground"))
                {
                    _health -= 2f;
                }
                //UNCOMMENT FOR INSTANT DEATH ON WALL BREAK
                //WILL PROBABLY LATER BE IMPLEMENTED INTO SEPARATE MODE
                //           if (_health < 0)
                //           {
                //                rb.velocity = collision.relativeVelocity;
                //           }

                //Destory damage at 0 health
                if (_health < 0)
                {
                    StopAllCoroutines();
                    if (!hasPlayedSFX[2])
                    {
                        _ms.playSFX(1);
                        hasPlayedSFX[2] = true;
                    }
                    Destroy(gameObject);
                }
                //At quarter health go to major damage
                else if (_health < 25)
                {
                    StopAllCoroutines();
                    StartCoroutine(wallPulsing(2, Color.red));
                    GetComponent<SpriteRenderer>().sprite = sprites[2];
                    if (gameObject.tag.Equals("Ceiling"))
                    {
                        gameObject.GetComponent<Animator>().SetBool("IsBroken", true);
                    }
                    if (!hasPlayedSFX[1])
                    {
                        _ms.playSFX(0);
                        hasPlayedSFX[1] = true;
                    }
                }
                //At half health go to minor damage
                else if (_health < 50)
                {
                    StopAllCoroutines();
                    StartCoroutine(wallPulsing(3, Color.yellow));
                    GetComponent<SpriteRenderer>().sprite = sprites[1];
                    if (gameObject.tag.Equals("Ceiling"))
                    {
                        gameObject.GetComponent<Animator>().SetBool("IsHurt", true);
                    }
                    if (!hasPlayedSFX[0])
                    {
                        _ms.playSFX(0);
                        hasPlayedSFX[0] = true;
                    }
                }

            }
            else
            {
                    ks = collision.gameObject.GetComponent<OnlineKnockbackScript>();
                    if (ks.movePercent < 1)
                    {
                    updateDamageServerRpc(onlineHealth.Value, onlineHealth.Value - (ks.getDamage() / 7f * (1.2f - ks.movePercent)));
                    }
                    else if (gameObject.tag.Equals("Ground"))
                    {
                    updateDamageServerRpc(onlineHealth.Value, onlineHealth.Value - .8f);
                    }
                    
                }
            }
        }
    [ServerRpc(RequireOwnership = false)]
    public void updateDamageServerRpc(float previous, float current)
    {
        onlineHealth.Value = current;
        if (onlineHealth.Value < 0)
        {
            hasPlayedSFX[2] = true;
            doBreakingClientRpc(2);
            if (!hasPlayedSFX[2])
            {
                _ms.playSFX(1);
            }
        }
        else if (onlineHealth.Value < 25)
        {
            hasPlayedSFX[1] = true;
            GetComponent<SpriteRenderer>().sprite = sprites[2];
            doBreakingClientRpc(1);
            if (!hasPlayedSFX[1])
            {
                _ms.playSFX(0);

            }

        }
        else if (onlineHealth.Value < 50)
        {
            hasPlayedSFX[0] = true;
            GetComponent<SpriteRenderer>().sprite = sprites[1];
            doBreakingClientRpc(0);
            if (!hasPlayedSFX[0])
            {
                _ms.playSFX(0);
            }

        }
    }
}
