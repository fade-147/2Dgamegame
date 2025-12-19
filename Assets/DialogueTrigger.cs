using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    private bool isPlayerInRange = false; // 标记玩家是否在触发范围内
    private bool FinishDialogue=false;
    // 碰撞体进入时，标记玩家在范围内
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("按E和" + gameObject.name + "对话");
        }
    }

    // 碰撞体离开时，重置状态
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    // 每一帧检测输入
    private void Update()
    {

        // 玩家在范围内  + 对话面板未激活
        if (isPlayerInRange     // && Keyboard.current.eKey.wasPressedThisFrame 
                                // 新系统检测E键按下
    && !DialogueManager.Instance.dialoguePanel.activeInHierarchy
    &&!FinishDialogue )
        {
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
