using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ItemType.cs - 物品类型枚举
public enum ItemType
{
    Normal,       // 普通物品（背包格子）
    MeleeWeapon,  // 近战武器
    RangedWeapon, // 远程武器
    Armor         // 防具
}
//[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("基础信息")]
    public string itemName = "新物品";
    [TextArea] public string description = "物品描述";
    public Sprite icon; // 物品图标
    public ItemType itemType; // 物品类型

    [Header("实体关联（武器/防具专属）")]
    public GameObject weaponPrefab; // 武器对应的实体预制体（比如你的枪模型）

    [Header("战斗属性")]
    public int damage; // 武器伤害
    public int defense; // 防具防御
}