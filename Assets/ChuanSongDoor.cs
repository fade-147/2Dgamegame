using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChuanSongDoor : MonoBehaviour
{
    public GameObject signSprite;
    public Transform playerTrans;
    private Animator anim;
    public Vector3 PosistionToGo;
    private bool isPlayerInRange = false; // 标记玩家是否在触发范围内
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerTrans = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            signSprite = GameObject.Find("Sign Spirte1");
            anim = signSprite.GetComponent<Animator>();
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

        // 玩家在范围内 
        if (isPlayerInRange && Keyboard.current.rKey.wasPressedThisFrame)
        {
            signSprite.GetComponent<SpriteRenderer>().enabled = false;
            anim.Play("keyboard");
            playerTrans.position = PosistionToGo;
        }
    }
}
