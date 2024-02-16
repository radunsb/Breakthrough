using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakScript : MonoBehaviour
{
    public GameObject _prefabRWall;
    public GameObject _prefabLWall;
    public GameObject _prefabCeiling;
    public GameObject _prefabFloor;
    Animator = H_Animator;

    int _health = 100;

    void Start()
    {
        GameObject rWall = Instantiate(_prefabRWall, new Vector2(10,0), Quaternion.identity);
        GameObject LWall = Instantiate(_prefabLWall, new Vector2(-10,0), Quaternion.identity);
        GameObject Ceiling = Instantiate(_prefabCeiling, new Vector2(0,5), Quaternion.identity);
        GameObject Floor = Instantiate(_prefabFloor, new Vector2(0, -4.3), Quaternion.identity);
    }

    void Update()
    {
        if(_health < 50){
            isHurt == true;
            H_Animator.SetBool("isHurt", true);
        }

        if (_health < 25)
        {
            isBroken == true;
            H_Animator.SetBool("isBroken", true);
        }

        if (_health <= 0)
        {
            destroy(GameObject RWALL);
        }
    }

    void onCollisionEnter(Collision col)
    {
        int force = col.impulse / timeDeltaTime;
        _health -= force;
    }
}
