using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sign : MonoBehaviour
{
    private PlayInputControl playerInput;
    private Animator anim;
    public Transform playerTrans;
    public GameObject signSprite;
    private IInteractable targetItem;
    private bool canPress;

    private void Awake()
    {
        //anim = GetComponentInChildren<Animator>();
        anim = signSprite .GetComponent<Animator>();

        playerInput =new PlayInputControl();
        playerInput.Enable();

    }

    private void OnEnable()
    {
        InputSystem.onActionChange += OnActionChange;
        playerInput.gamePlayer.Confirm.started += OnConfirm;
    }

    private void OnDisable()
    {
        canPress = false;
    }

    private void Update()
    {
        signSprite .GetComponent<SpriteRenderer >().enabled = canPress;
        signSprite.transform .localScale =playerTrans.localScale;
    }

    private void OnConfirm(InputAction.CallbackContext obj)
    {
        if (canPress)
        {
            targetItem.TriggerAction();
            GetComponent<AudioDefination>()?.PlayAudioClip();
            canPress = false;
        }
    }

    //设备输入时切换
    private void OnActionChange(object obj, InputActionChange actionChange)
    {
        if (actionChange == InputActionChange.ActionStarted)
        {
            var d = ((InputAction)obj).activeControl.device;

            switch (d.device)
            {
                case Keyboard:
                    anim.Play("keyboard");
                    break;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("interactable"))
        {
            canPress = true;
            targetItem =other.GetComponent<IInteractable >();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("interactable"))
        {
            canPress = false;
        }
    }

}
