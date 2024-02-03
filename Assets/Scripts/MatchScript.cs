using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchScript : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        // will eventually open a pause menu
        if (Input.GetKeyDown(KeyCode.Escape)){
            SceneManager.LoadScene("TitleScene");
        }
    }
}
