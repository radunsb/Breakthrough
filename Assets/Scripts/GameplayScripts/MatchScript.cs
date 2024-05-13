using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Netcode;

public class MatchScript : NetworkBehaviour
{
    public AudioSource _as;
    public AudioSource _asSFX;
    public AudioClip[] sfx;

    VictoryScript _victoryScript;
    public int roundsToWin;
    public GameObject[] possiblePlayers;
    public GameObject[] possibleSandbags;
    public GameObject[] possibleEnemies;
    public GameObject[] icons;
    public Text p1winsText;
    public Text p2winsText;
    public int[] matchInfo;
    GameObject p1;
    GameObject p2;
    GameObject currentWorld;
    //GameObject floor;

    void Start()
    {
        _as.volume = (PlayerPrefs.HasKey("Volume")) ? PlayerPrefs.GetFloat("Volume") : 1.0f;
        _asSFX.volume = (PlayerPrefs.HasKey("Volume")) ? PlayerPrefs.GetFloat("Volume") : 1.0f;
        matchInfo = new int[3];
        if (PlayerPrefs.GetString("Match Type") == "2 Player Local")
        {
            //Make playerOne a PLAYER, set to left side
            GameObject playerOne = Instantiate(possiblePlayers[PlayerPrefs.GetInt("Player1Char")],
            new Vector2(-4f, -3f), Quaternion.identity);
            //make playerTwo a PLAYER, set to right side
            GameObject playerTwo = Instantiate(possiblePlayers[PlayerPrefs.GetInt("Player2Char")],
                new Vector2(4f, -3f), Quaternion.identity);
            //Set playerIndex of both players
            icons[PlayerPrefs.GetInt("Player1Char")].SetActive(true);
            icons[PlayerPrefs.GetInt("Player2Char") + 4].SetActive(true);
            playerOne.GetComponent<PlayerScript>().playerIndex = 0;
            playerTwo.GetComponent<PlayerScript>().playerIndex = 1;
            p1winsText.text = "Player one wins: " + PlayerPrefs.GetInt("P1 Points");
            p2winsText.text = "Player two wins: " + PlayerPrefs.GetInt("P2 Points");
            //Set gameObjects for each entity
            p1 = playerOne;
            p2 = playerTwo;
            p1.GetComponent<PlayerScript>().triangle.GetComponent<SpriteRenderer>().color = Color.cyan;
            p2.GetComponent<PlayerScript>().triangle.GetComponent<SpriteRenderer>().color = new Color(1f, 0.4f, 0f);
        }
        else if (PlayerPrefs.GetString("Match Type") == "Training")
        {
            //Make playerOne a PLAYER, set to left side
            GameObject playerOne = Instantiate(possiblePlayers[PlayerPrefs.GetInt("Player1Char")],
            new Vector2(-4f, -3f), Quaternion.identity);
            //Set playerIndex
            playerOne.GetComponent<PlayerScript>().playerIndex = 0;
            //make sandbagOne a SANDBAG, set to right side
            GameObject sandbagOne = Instantiate(possibleSandbags[PlayerPrefs.GetInt("Player2Char")],
                new Vector2(4f, -3f), Quaternion.identity);
            //Set the sandbag's opponent, since its knockBack script wont do it itself
            sandbagOne.GetComponent<KnockbackScript>().setOpponent(playerOne);

            p1winsText.text = "Player one wins: " + PlayerPrefs.GetInt("P1 Points");
            p2winsText.text = "Player two wins: " + PlayerPrefs.GetInt("P2 Points");
            icons[PlayerPrefs.GetInt("Player1Char")].SetActive(true);
            icons[4].SetActive(true);
            //set gameObjects for each entity
            p1 = playerOne;
            p2 = sandbagOne;
            p1.GetComponent<PlayerScript>().triangle.GetComponent<SpriteRenderer>().color = Color.cyan;
        }
        else if (PlayerPrefs.GetString("Match Type") == "1 Player Local")
        {
            //Make playerOne a PLAYER, set to left side
            GameObject playerOne = Instantiate(possiblePlayers[PlayerPrefs.GetInt("Player1Char")],
            new Vector2(-4f, -3f), Quaternion.identity);
            //Set playerIndex
            playerOne.GetComponent<PlayerScript>().playerIndex = 0;
            //make sandbagOne a SANDBAG, set to right side
            GameObject enemyOne = Instantiate(possibleEnemies[PlayerPrefs.GetInt("Player2Char")],
                new Vector2(4f, -3f), Quaternion.identity);
            //Set the sandbag's opponent, since its knockBack script wont do it itself
            enemyOne.GetComponent<KnockbackScript>().setOpponent(playerOne);
            enemyOne.GetComponent<AIScript>().playerIndex = 1;
            p1winsText.text = "Player one wins: " + PlayerPrefs.GetInt("P1 Points");
            p2winsText.text = "Player two wins: " + PlayerPrefs.GetInt("P2 Points");
            icons[PlayerPrefs.GetInt("Player1Char")].SetActive(true);
            icons[PlayerPrefs.GetInt("Player2Char") + 4].SetActive(true);
            //set gameObjects for each entity
            p1 = playerOne;
            p2 = enemyOne;
            p1.GetComponent<PlayerScript>().triangle.GetComponent<SpriteRenderer>().color = Color.cyan;
        }
        matchInfo[0] = PlayerPrefs.GetInt("Player1Char");
        matchInfo[1] = PlayerPrefs.GetInt("Player2Char");
        matchInfo[2] = PlayerPrefs.GetInt("Stage");
        //Should be updated for the other main stages
        if (matchInfo[2] == 4)
        {
            currentWorld = GameObject.Find("House (Main)");
        }
        else if (matchInfo[2] == 5)
        {
            currentWorld = GameObject.Find("Subway (Main)");
        }
        else
        {
            currentWorld = GameObject.Find("Tower (Main)");
        }
        p1winsText.color = Color.cyan;
        p2winsText.color = new Color(1f, 0.4f, 0f);
        

    }

    public void playSFX(int sfxID)
    {
        _asSFX.PlayOneShot(sfx[sfxID]);
    }
    public virtual void updateCharacterPoints(int playerIndex, GameObject worldToSpawn)
    {
        int newPoints = 0;
        if(playerIndex == 0)
        {
            newPoints = PlayerPrefs.GetInt("P1 Points") + 1;
            PlayerPrefs.SetInt("P1 Points", newPoints);
        }
        else if(playerIndex == 1)
        {
            newPoints = PlayerPrefs.GetInt("P2 Points") + 1;
            PlayerPrefs.SetInt("P2 Points", newPoints);
        }
        if(newPoints >= roundsToWin)
        {
            matchOver(playerIndex);
        }
        else
        {
            StartCoroutine(roundOver(playerIndex, worldToSpawn));
        }
    }

    protected virtual IEnumerator roundOver(int winningPlayerIndex, GameObject worldToSpawn)
    {
        transition();
        yield return new WaitForSeconds(5);
        
        //Destroy the current background and walls
        Destroy(currentWorld);
        //Reset player one
        //Instantiate the new background and walls
        currentWorld = Instantiate(worldToSpawn);


        p1.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        p1.transform.position = new Vector2(-4, -2);
        p1.GetComponent<KnockbackScript>().setDamage(0);
        //Reset player two
        p2.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        p2.transform.position = new Vector2(4, -2);
        p2.GetComponent<KnockbackScript>().setDamage(0);
        p1winsText.text = "Player one wins: " + PlayerPrefs.GetInt("P1 Points");
        p2winsText.text = "Player two wins: " + PlayerPrefs.GetInt("P2 Points");

    }

    void transition()
    {
        RaycastHit2D Rhit = Physics2D.Raycast(transform.position, Vector2.right);
        float Rdist = Mathf.Abs(Rhit.point.y - transform.position.y);
        print(Rdist);
        RaycastHit2D Lhit = Physics2D.Raycast(transform.position, Vector2.left);
        float Ldist = Mathf.Abs(Lhit.point.y - transform.position.y);
        RaycastHit2D Dhit = Physics2D.Raycast(transform.position, -Vector2.up);
        float Ddist = Mathf.Abs(Dhit.point.y - transform.position.y);
        RaycastHit2D Uhit = Physics2D.Raycast(transform.position, Vector2.up);
        float Udist = Mathf.Abs(Uhit.point.y - transform.position.y);

    }

    void matchOver(int winningPlayerIndex)
    {
        SceneManager.LoadScene("WinScene");
    }
    public int getStage()
    {
        return matchInfo[2];
    }
}
