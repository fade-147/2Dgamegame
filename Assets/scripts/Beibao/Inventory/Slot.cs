using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{


    //让背包界面的格子获得对应物品的基本属性
    public enum SlotType
    {
        BagSlot,           // 背包普通槽
        MeleeWeaponSlot,   // 近战武器槽
        RangedWeaponSlot,  // 远程武器槽
        FangyuSlot
    }

    public int slotID;  //空格ID等于物品ID
    public Item slotItem;
    public Image slotImage;
    public Text slotNum;
    public string slotInfo;
    public SlotType slotType;  //设置当前格子的类型

    public GameObject itemInSlot;
    public void ItemOnClicked()
    {
        InventoryManager.UpdateItemInfo(slotInfo);
        InventoryManager.instance.SelectSlot(this);
    }
    public void SetupSlot(Item item)
    {
        if (item == null)
        {                 //一开始我们已经设置了15个格子里有空的，我们不想显示出来大白格子
            itemInSlot.SetActive(false);
            return;
        }
        itemInSlot.SetActive(true);
        //如果不是空的话，那就把基本信息同步到格子里
        slotImage.sprite = item.itemImage;
        slotItem = item;

        //武器槽不需要显示数量
        if (slotType == SlotType.BagSlot)
        {
            slotNum.text = item.itemHeld.ToString();
        }
        else
        {
            slotNum.text = ""; // 武器槽隐藏数量
        }
        slotInfo = item.itemInfo;
    }
}
