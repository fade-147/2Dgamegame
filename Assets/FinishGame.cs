using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FinishGame : MonoBehaviour
{
    private bool isPlayerInRange = false; // 标记玩家是否在触发范围内
    public GameObject FinishPanel;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;

        }
    }
    private void Update()
    {

        // 玩家在范围内  + 对话面板未激活
        if (isPlayerInRange     && Keyboard.current.rKey.wasPressedThisFrame )
        {
            FinishPanel .SetActive(true);
        }
    }
}
