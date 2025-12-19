using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShangDian : MonoBehaviour
{
    private bool isPlayerInRange = false; // 标记玩家是否在触发范围内
    private Animator anim;
    public Transform playerTrans;
    private GameObject signSprite;
    public GameObject ShangDianPanel;

    public GameObject NormalText;
    public GameObject OKText;
    public GameObject NoMoneyText;
    public GameObject DieText;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerTrans = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            signSprite = GameObject.Find("Sign Spirte1");
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            signSprite.GetComponent<SpriteRenderer>().enabled = true;

        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            signSprite.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
    private void Update()
    {
        if (signSprite != null && playerTrans != null)
        {
            signSprite.transform.localScale = playerTrans.localScale;
        }
        // 玩家在范围内  + 对话面板未激活
        if (isPlayerInRange && Keyboard.current.rKey.wasPressedThisFrame)
        {
            signSprite.GetComponent<SpriteRenderer>().enabled = false;
            ShangDianPanel.SetActive(true);
            NormalText.SetActive(true);
            OKText.SetActive(false);
            NoMoneyText.SetActive (false);
            DieText.SetActive(false);
        }
    }
}
