using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakScript : MonoBehaviour
{
    public GameObject _prefabRWall;
    public GameObject _prefabLWall;
    public GameObject _prefabCeiling;
    public GameObject _prefabFloor;
    Animator H_Animator;

    int _health = 100;
    bool isNew = true;
    bool isHurt = false;
    bool isBroken = false;

    void Start()
    {
        GameObject RWall = Instantiate(_prefabRWall, new Vector2(-11.6f,0), Quaternion.identity);
        GameObject LWall = Instantiate(_prefabLWall, new Vector2(11.6f,0), Quaternion.identity);
        GameObject Ceiling = Instantiate(_prefabCeiling, new Vector2(0,4.25f), Quaternion.identity);
        GameObject Floor = Instantiate(_prefabFloor, new Vector2(0, -4.3f), Quaternion.identity);
        H_Animator = gameObject.GetComponent<Animator>();
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
    void onCollisionEnter(Collision col)
    {
     //   int force = col.impulse / Time.deltaTime;
     //   _health -= force;
    }
}
