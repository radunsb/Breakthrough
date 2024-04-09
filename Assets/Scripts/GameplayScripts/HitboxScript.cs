using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class HitboxScript : MonoBehaviour
{
    //Standard range: 1.4 - 2.4
    public float velocityMult;
    //Straight right = 0, up = 90, left = 180, etc
    public float launchDirection;

    public int damage;
}
