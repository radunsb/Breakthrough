using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]

public class BreakScript : MonoBehaviour
{
    
    float _health = 100;
    //List of healthy and damaged sprites
    public Sprite[] sprites;
    //Colors to indicate damage and breakability
    Color ColorStart = new Color(0,0,0);
    Color ColorEnd = new Color(0,255,0);
    Color ColorHurt = new Color(255, 165, 0);
    Color ColorBroke = new Color(255,0,0);
    float i = 0;
    MatchScript _ms;
    bool[] hasPlayedSFX = { false, false, false };
    bool isOnline;
    NetworkVariable<float> onlineHealth = new NetworkVariable<float>(100);

 private void Start()
    {
        isOnline = PlayerPrefs.GetString("Match Type") == "2 Player Online";
        _ms = GameObject.FindObjectOfType<MatchScript>();
    }


    void Update()
    { 
        if(!isOnline){
        gameObject.GetComponent<SpriteRenderer>().color = ColorEnd;
        //i = i + Time.deltaTime;
        //SpriteRenderer.material.color = Color.Lerp(ColorStart, ColorEnd, Mathf.PingPong(i * 2, 1));
        //if (i >= 1)
        //{
        //    i = 0;
        //    SpriteRenderer.material.color = Color.Lerp(ColorEnd, ColorStart, Mathf.PingPong(i * 2, 1));
        //}
            //At half health go to minor damage
            if (_health < 50)
        {
            gameObject.GetComponent<SpriteRenderer>().color = ColorHurt;
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
        //At quarter health go to major damage
        if (_health < 25)
        {
            gameObject.GetComponent<SpriteRenderer>().color = ColorBroke;
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
        //Destory damage at 0 health
        if(_health < 0)
        {
             if (!hasPlayedSFX[2])
                {
                    _ms.playSFX(1);
                    hasPlayedSFX[2] = true;
                }
            Destroy(gameObject);
        }
    }
    else
        {
            if (IsServer) {
                if (onlineHealth.Value < 50)
                {
                    if (!hasPlayedSFX[0])
                    {
                        _ms.playSFX(0);
                        hasPlayedSFX[0] = true;
                    }
                    GetComponent<SpriteRenderer>().sprite = sprites[1];
                    doBreakingClientRpc(0);
                }
                if(onlineHealth.Value < 25)
                {
                    if (!hasPlayedSFX[1])
                    {
                        _ms.playSFX(0);
                        hasPlayedSFX[1] = true;
                    }
                    GetComponent<SpriteRenderer>().sprite = sprites[2];
                    doBreakingClientRpc(1);
                }
                if(onlineHealth.Value < 0)
                {
                    if (!hasPlayedSFX[2])
                    {
                        _ms.playSFX(1);
                        hasPlayedSFX[2] = true;
                    }
                    Destroy(gameObject);
                    doBreakingClientRpc(2);
                }
            }
        }

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
                Destroy(gameObject);
                break;
            default: break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player" || collision.gameObject.tag == "Sandbag")
        {
            if(!isOnline){
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            KnockbackScript ks = collision.gameObject.GetComponent<KnockbackScript>();
            //Makes sure player impacting is actively being knocked back
            //Allows us to avoid having floor take damage from jumping, etc.
            if (ks.movePercent < 1)
            {
                _health -= (ks.getDamage()/8f * (1.2f - ks.movePercent));
            }
            else if (gameObject.tag.Equals("Ground"))
            {
                _health -= 1;
            }
            //UNCOMMENT FOR INSTANT DEATH ON WALL BREAK
            //WILL PROBABLY LATER BE IMPLEMENTED INTO SEPARATE MODE
//           if (_health < 0)
//           {
//                rb.velocity = collision.relativeVelocity;
//           }

        }
        else
            {
                if (IsServer)
                {
                    KnockbackScript ks = collision.gameObject.GetComponent<KnockbackScript>();
                    if(ks.movePercent < 1)
                    {
                        onlineHealth.Value -= (ks.getDamage() / 7f * (1.2f - ks.movePercent));
                    }
                    else if (gameObject.tag.Equals("Ground"))
                    {
                        onlineHealth.Value -= .8f;
                    }
                }
            }


    }
}
