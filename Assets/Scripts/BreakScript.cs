using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakScript : MonoBehaviour
{
    
    float _health = 100;
    //List of healthy and damaged sprites
    public Sprite[] sprites;


    void Update()
    {
        //At half health go to minor damage
        if (_health < 50)
        {
            GetComponent<SpriteRenderer>().sprite = sprites[1];
        }
        //At quarter health go to major damage
        if (_health < 25)
        {
            GetComponent<SpriteRenderer>().sprite = sprites[2];
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
            if (ks.movePercent < 1 || ks.getInKnockback())
            {
                //Vertical walls should only take y velocity, otherwise would
                //not work in an intuitive way
                if (gameObject.tag.Equals("Vertical Wall"))
                {
                    _health -= ((Mathf.Abs(rb.velocity.y)));
                }
                //Horizontal walls add horizontal and vertical velocity
                else
                {
                    _health -= ((Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.y)));
                }
            }
        }
        //DEBUG
        print(_health);
    }
}
