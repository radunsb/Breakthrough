using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyScript : MonoBehaviour
{
    bool _inKnockback;
    float _damage;
    Rigidbody2D _rbody;
    public GameObject opponent;
    PlayerScript _opponentScript;
    // Start is called before the first frame update
    void Start()
    {
        _inKnockback = false;
        _damage = 0;
        _rbody = GetComponent<Rigidbody2D>();
        _opponentScript = opponent.GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    float calcLaunchMultiplier(float velocityMult)
    {
        return velocityMult * (_damage / 50 + 1) * 200;
    }

    void takeKnockback(float velocityMult, float launchDirection)
    {
        float lm = calcLaunchMultiplier(velocityMult);
        launchDirection = launchDirection * Mathf.Deg2Rad;
        Vector2 force = new Vector2(Mathf.Cos(launchDirection) * lm, Mathf.Sin(launchDirection) * lm);
        _rbody.velocity = Vector2.zero;
        _rbody.AddForce(force);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Hitbox"))
        {
            if (!_inKnockback)
            {
                HitboxScript hs = collision.gameObject.GetComponent<HitboxScript>();
                float ld = hs.launchDirection;
                if (_opponentScript._flipX)
                {
                    ld = 180 - ld;
                }
                takeKnockback(hs.velocityMult, ld);
                _inKnockback = true;
                Invoke("allowKnockback", 0.2f);
            }
        }
    }

    void allowKnockback()
    {
        _inKnockback = false;
    }
}
