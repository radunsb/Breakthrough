using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Update()
    {
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
        }
        //Destory damage at 0 health
        if(_health < 0)
        {
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

    }
}
