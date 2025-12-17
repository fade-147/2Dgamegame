using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SavePoint : MonoBehaviour,IInteractable   //接口
{
    [Header("广播")]
    public VoidEventSO saveDataEvent;

    [Header("变量参数")]
    public  SpriteRenderer spriteRenderer;

    public bool isDone;
    public GameObject SaveSprite;
    public Sprite lightSprite;

    private void OnEnable()
    {
        SaveSprite.SetActive(isDone);
    }

    public void TriggerAction()
    {
        if (!isDone)
        {
            isDone = true;
            //spriteRenderer.sprite = lightSprite;
            SaveSprite.SetActive(true);
            saveDataEvent.RaiseEvent();

            this.gameObject.tag = "Untagged";
        }
    }
}