using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakScript : MonoBehaviour
{
    
    float _health = 100;
    public Sprite[] sprites;

    void Start()
    {
    }

    void Update()
    {
        if (_health < 50)
        {
            GetComponent<SpriteRenderer>().sprite = sprites[1];
        }

        if (_health < 25)
        {
            GetComponent<SpriteRenderer>().sprite = sprites[2];
        }

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
            if (ks.movePercent < 1 || ks.getInKnockback())
            {
                if (gameObject.tag.Equals("Vertical Wall"))
                {
                    _health -= ((Mathf.Abs(rb.velocity.y)));
                }
                else
                {
                    _health -= ((Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.y)));
                }
            }
        }
        print(_health);
    }
}
