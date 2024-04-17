using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryScript : MonoBehaviour
{
    //Prefab of background, walls, and boundaries of the stage that spawns on contact
    public GameObject worldToSpawn;
    public MatchScript _ms;
    //prevents scene activating twice if two people fall into floor
    public bool hasBeenActivated;

    void Start()
    {
        _ms = GameObject.FindObjectOfType<MatchScript>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasBeenActivated)
        {
            //Player is always index 0 in training mode, so give player 0 points
            if (collision.gameObject.tag.Equals("Sandbag"))
            {
                GameObject[] boundaries = GameObject.FindGameObjectsWithTag("Boundary");
                foreach (GameObject boundary in boundaries)
                {
                    boundary.GetComponent<BoundaryScript>().hasBeenActivated = true;
                }

                _ms.updateCharacterPoints(0, worldToSpawn);
            }

            else if (collision.gameObject.tag.Equals("Player"))
            {
                GameObject[] boundaries = GameObject.FindGameObjectsWithTag("Boundary");
                foreach (GameObject boundary in boundaries)
                {
                    boundary.GetComponent<BoundaryScript>().hasBeenActivated = true;
                }

                PlayerScript _ps = collision.gameObject.GetComponent<PlayerScript>();
                //get player index of the player NOT pushed out of bounds
                int playerIndex = (_ps.playerIndex + 1) % 2;
                //tell manager script to update score and spawn next round
                _ms.updateCharacterPoints(playerIndex, worldToSpawn);
            }
        }
    }
}
