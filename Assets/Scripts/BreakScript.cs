using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakScript : MonoBehaviour
{
    Rigidbody2D oneRB;
    Rigidbody2D twoRB;
    public GameObject CharacterOne;
    public GameObject CharacterTwo;
    public GameObject _prefabRWall;
    public GameObject _prefabLWall;
    public GameObject _prefabCeiling;
    public GameObject _prefabFloor;
    Animator H_Animator;

    float _health = 100;
    bool isNew = true;
    bool isHurt = false;
    bool isBroken = false;

    void Start()
    {
        oneRB = CharacterOne.GetComponent<Rigidbody2D>();
        twoRB = CharacterTwo.GetComponent<Rigidbody2D>();
 //       GameObject RWall = Instantiate(_prefabRWall, new Vector2(-8,0), Quaternion.identity);
 //       RWall.SetActive(true);
 //       GameObject LWall = Instantiate(_prefabLWall, new Vector2(8,0), Quaternion.identity);
 //       LWall.SetActive(true);
 //       GameObject Ceiling = Instantiate(_prefabCeiling, new Vector2(0,4.25f), Quaternion.identity);
 //       Ceiling.SetActive(true);
 //       GameObject Floor = Instantiate(_prefabFloor, new Vector2(0, -4.3f), Quaternion.identity);
 //       Floor.SetActive(true);
 //       H_Animator = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        if (_health < 50)
        {
            isHurt = true;
            H_Animator.SetBool("isHurt", true);
        }

        if (_health < 25)
        {
            isBroken = true;
            H_Animator.SetBool("isBroken", true);
        }

 //       if (RWall._health <= 0)
//        {
//            Destroy(rWall);
//        }
//        if (LWall._health <= 0)
//        {
//            Destroy(lWall);
//        }
//        if (Ceiling._health <= 0)
//        {
//            Destroy(Ceiling);
//        }
//        if (Floor._health <= 0)
//        {
//           Destroy(Floor);
//        }


    }
    void onCollisionEnter2D(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            float oneVel = (oneRB.velocity.x + oneRB.velocity.y) / 2;
            _health -= oneVel;
        }
        if (col.gameObject.tag == "Sandbag")
        {
            float twoVel = (twoRB.velocity.x + twoRB.velocity.y) / 2;
            _health -= twoVel;
        }
    }
}
