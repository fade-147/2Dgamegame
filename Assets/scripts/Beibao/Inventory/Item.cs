using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/New Item")]
public class Item : ScriptableObject
{   //单个物品的基本属性
    // ItemType.cs - 物品类型枚举
    public enum ItemType
    {
        Normal,       // 普通物品（背包格子）
        MeleeWeapon,  // 近战武器
        RangedWeapon, // 远程武器
        Fangyu        // 防具
    }

    public string itemName;     //名字
    public Sprite itemImage;    //图像
    public int itemHeld;         //数量
    [TextArea]
    public string itemInfo;        //下面的描述
    public ItemType itemType;     //类型描述
}