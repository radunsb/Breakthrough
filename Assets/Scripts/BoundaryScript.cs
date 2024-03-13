using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryScript : MonoBehaviour
{
    public GameObject worldToSpawn;
    public MatchScript _ms;
    // Start is called before the first frame update
    void Start()
    {
        _ms = GameObject.FindObjectOfType<MatchScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Sandbag"))
        {
            _ms.updateCharacterPoints(0, worldToSpawn);
        }

        else if (collision.gameObject.tag.Equals("Player")){
            PlayerScript _ps = collision.gameObject.GetComponent<PlayerScript>();
            //get player index of the player NOT pushed out of bounds
            int playerIndex = (_ps.playerIndex + 1) % 2;
            _ms.updateCharacterPoints(playerIndex, worldToSpawn);
        }
    }
}
