using System.Collections;
using System.Collections.Generic;
//using System.Runtime.Remoting.Activation;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using static System.Net.Mime.MediaTypeNames;

[RequireComponent(typeof(PlayerInput))]

public class OptionsScript : TitleScript
{
    bool _inSubMenu = false;
    bool _inVolume = false;
    public GameObject volumeBar;
    public GameObject volumeSettings;
    float volume;
    protected override void Start()
    {
        base.Start();
        volume = (PlayerPrefs.HasKey("Volume") ? PlayerPrefs.GetFloat("Volume") : 1);
    }
    protected override void navigate(InputAction.CallbackContext context)
    {
        Vector2 directions = context.ReadValue<Vector2>();
        if (!_inSubMenu)
        {
            if (directions.y > 0.5)
            {
                //Scroll one button up
                updateActiveButton(-1);
            }
            else if (directions.y < -0.5)
            {
                //Scroll one button down
                updateActiveButton(1);
            }
        }
        else if (_inVolume)
        {
            if (volume > 1)
            {
                volume = 1;
            }
            else if (volume < 0)
            {
                volume = 0;
            }
            if (directions.x > 0.5 && volume < 1)
            {
                volume += 0.05f;                         
            }
            else if(directions.x < -0.5 && volume > 0)
            {
                volume -= 0.05f;
            }
            volumeBar.transform.localScale = new Vector2(volume, 1);
        }

    }
    public override void processBackInput(InputAction.CallbackContext context)
    {      
        if (_inSubMenu)
        {
            _inSubMenu = false;
            _inVolume = false;
            volumeSettings.SetActive(false);
            PlayerPrefs.SetFloat("Volume", volume);
        }
        else
        {
            SceneManager.LoadScene("TitleScene");
        }
    }

    protected override void processSelectInput(InputAction.CallbackContext context)
    {
        int _buttonActive = this._buttonActive;
        switch (_buttonActive)
        {
            case 0:
                _inSubMenu = true;
                _inVolume = true;
                volumeBar.transform.localScale = new Vector2(volume, 1);
                volumeSettings.SetActive(true);
                break; //Volume
            case 1:
                SceneManager.LoadScene("ControlsScene");
                break;
            case 2:
                SceneManager.LoadScene("TitleScene");
                break;
            default:
                break;

        }
    }
}
