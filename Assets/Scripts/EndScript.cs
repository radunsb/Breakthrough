using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndScript : MonoBehaviour
{
    void Start()
    {
        if (Input.GetKeyDown("enter"))
        {
            SceneManager.LoadScene("TitleScene");
        }
    }

        void Update()
    {
        
    }
}
