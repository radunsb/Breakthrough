using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]

public class BreakScript : MonoBehaviour
{
    
    float _health = 100;
    //List of healthy and damaged sprites
    public Sprite[] sprites;
    MatchScript _ms;
    bool[] hasPlayedSFX = { false, false, false };

    private void Start()
    {
        _ms = GameObject.FindObjectOfType<MatchScript>();

    }

    void Update()
    {
        //At half health go to minor damage
        if (_health < 50)
        {
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player" || collision.gameObject.tag == "Sandbag")
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            KnockbackScript ks = collision.gameObject.GetComponent<KnockbackScript>();
            //Makes sure player impacting is actively being knocked back
            //Allows us to avoid having floor take damage from jumping, etc.
            if (ks.movePercent < 1)
            {
                _health -= (ks.getDamage()/7f * (1.2f - ks.movePercent));
            }
            else if (gameObject.tag.Equals("Ground"))
            {
                _health -= .8f;
            }
            //UNCOMMENT FOR INSTANT DEATH ON WALL BREAK
            //WILL PROBABLY LATER BE IMPLEMENTED INTO SEPARATE MODE
//           if (_health < 0)
//           {
//                rb.velocity = collision.relativeVelocity;
//           }
        }

    }
}
