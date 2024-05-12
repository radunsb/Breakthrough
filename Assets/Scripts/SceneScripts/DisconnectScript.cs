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
        Color col = GetComponent<SpriteRenderer>().color;
        float newA = GetComponent<SpriteRenderer>().color.a - 0.0005f;
        GetComponent<SpriteRenderer>().color = new Color(col.r, col.g, col.b, newA);
        if(GetComponent<SpriteRenderer>().color.a < .1f)
        {
            GameObject rick = GameObject.FindGameObjectWithTag("Rick");
            if (rick.GetComponent<SpriteRenderer>().color.a < 0.2f)
            {
                Color rickCol = rick.GetComponent<SpriteRenderer>().color;
                float rickNewA = rick.GetComponent<SpriteRenderer>().color.a + 0.0005f;
                rick.GetComponent<SpriteRenderer>().color = new Color(rickCol.r, rickCol.g, rickCol.b, rickNewA);

            }
        }
    }

    void processBackInput(InputAction.CallbackContext context) {
        SceneManager.LoadScene("TitleScene");
    }
}
