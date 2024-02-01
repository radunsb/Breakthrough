using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Animator))]
public class PlayerScript : MonoBehaviour
{

    public PlayControls controls;
    private InputAction normButton;
    Animator animator;

    private void Awake()
    {
        controls = new PlayControls();
    }
    private void OnEnable()
    {
        normButton = controls.Just_Attacks.Normal_Button;
        normButton.performed += normalButton;
        normButton.Enable();
        
    }

    private void OnDisable()
    {
        normButton.Disable();
    }
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void normalButton(InputAction.CallbackContext context)
    {
        animator.SetTrigger("Normal button");
    }
}
