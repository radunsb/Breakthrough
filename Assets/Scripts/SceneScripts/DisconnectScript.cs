using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DisconnectScript : MonoBehaviour
{
    AudioSource _as;
    public PlayControls controls;
    private InputAction back;

    private void Awake()
    {
        controls = new PlayControls();
    }

    protected virtual void OnEnable()
    {

        back = controls.Menus.Back;
        back.performed += processBackInput;
        back.Enable();
    }
    private void OnDisable()
    {
        back.performed -= processBackInput;
        back.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        _as = GetComponent<AudioSource>();
        _as.volume = (PlayerPrefs.HasKey("Volume")) ? PlayerPrefs.GetFloat("Volume") : 1.0f;
    }

    private void FixedUpdate()
    {
        transform.Rotate(0, 0, 0.02f);
        GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color.WithAlpha(GetComponent<SpriteRenderer>().color.a - 0.0005f);
        if(GetComponent<SpriteRenderer>().color.a < .1f)
        {
            GameObject rick = GameObject.FindGameObjectWithTag("Rick");
            if (rick.GetComponent<SpriteRenderer>().color.a < 0.5f)
            {
                rick.GetComponent<SpriteRenderer>().color = rick.GetComponent<SpriteRenderer>().color.WithAlpha(rick.GetComponent<SpriteRenderer>().color.a + 0.001f);
            }
        }
    }

    void processBackInput(InputAction.CallbackContext context) {
        SceneManager.LoadScene("TitleScene");
    }
}
