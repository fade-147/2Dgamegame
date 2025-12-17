using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour,IInteractable 
{
    private SpriteRenderer spriteRenderer;
    public Sprite openSprite;
    public Sprite closeSprite;
    public bool isDone;
    private PickupSpawner pickupSpawner;   //掉落道具脚本的引用

    private void Awake()
    {
        spriteRenderer =GetComponent<SpriteRenderer>();
        pickupSpawner = GetComponent<PickupSpawner>();
    }

    private void OnEnable()
    {
        spriteRenderer .sprite =isDone?openSprite : closeSprite;
    }
    public void TriggerAction()
    {
        Debug.Log("open");
        if (!isDone)
        {
            OpenChest();
        }
    }

    public void OpenChest()
    {
        spriteRenderer.sprite = openSprite;
        isDone = true;
        pickupSpawner.DropItems();
        this.gameObject.tag = "Untagged";
    }
}
