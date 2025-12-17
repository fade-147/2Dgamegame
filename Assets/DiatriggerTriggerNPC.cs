using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueTriggerNPC : MonoBehaviour
{
    public Dialogue dialogue; // 拖入你创建的Dialogue资源
    private bool isPlayerInRange = false; // 标记玩家是否在触发范围内
    private bool FinishDialogue = false;
    public GameObject signSprite;
    private Animator anim;
    public Transform playerTrans;

    // 碰撞体进入时，标记玩家在范围内
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerTrans =GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            signSprite = GameObject.Find("Sign Spirte1");
            anim = signSprite.GetComponent<Animator>();
            isPlayerInRange = true;
            signSprite.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    // 碰撞体离开时，重置状态
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            signSprite.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    // 每一帧检测输入
    private void Update()
    {
        if(signSprite  != null&&playerTrans !=null)
        {
            signSprite.transform.localScale = playerTrans.localScale;
        }
    
        // 玩家在范围内  + 对话面板未激活
        if (isPlayerInRange     && Keyboard.current.rKey.wasPressedThisFrame 
                                // 新系统检测E键按下
    && !DialogueManager.Instance.dialoguePanel.activeInHierarchy
    && !FinishDialogue)
        {
            signSprite.GetComponent<SpriteRenderer>().enabled = false;
            anim.Play("keyboard");
            TriggerDialogue();
        }
    }

    // 触发对话
    public void TriggerDialogue()
    {
        FinishDialogue = true;
        DialogueManager.Instance.StartDialogue(dialogue);
    }
}

