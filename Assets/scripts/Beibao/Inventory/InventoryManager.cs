using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public Inventory myBag;     //需要自己的背包
    public Inventory jinSlot;    //自己的装备背包
    public GameObject slotGrid;   //背包界面的每一个格子，就能按照格式一个一个的生成出来
    //public Slot slotPrefab;
    public GameObject emptySlot;
    public Text itemInfromation;    //用于描述的文字

    public Slot meleeWeaponSlot; // 近战武器槽
    public Slot rangedWeaponSlot; // 远程武器槽
    public Slot fangyuSlot; // 防御槽
    public GameObject meleeWeaponSlotPrefab;   // 近战武器框预制体
    public GameObject rangedWeaponSlotPrefab;  // 远程武器框预制体
    public GameObject fangyuPrefab;  // 防御预制体
    // 存储当前装备的武器
    public Item currentMeleeWeapon; // 当前近战武器
    public Item currentRangedWeapon; // 当前远程武器
    public Item currentfangyuWeapon; // 当前防御
    // 按钮引用
    public Button equipButton;
    public Button unequipButton;
    public Button deleteButton;
    private Slot currentSelectedSlot; // 当前选中的格子

    public List<GameObject> slots = new List<GameObject>();

    //武器变化事件，一旦武器栏的武器变化，那么就调用这个事件
    public delegate void WeaponChangedEvent(Item currentMelee, Item currentRanged,Item currentfangyuWeapon);
    public event WeaponChangedEvent OnWeaponChanged;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject); // 销毁整个Manager物体，避免残留无效组件
            return; // 终止后续代码，防止赋值无效的instance
        }
        // 给按钮添加点击事件
        //equipButton.onClick.AddListener(OnEquipButtonClicked);
        //unequipButton.onClick.AddListener(OnUnequipButtonClicked);
        //deleteButton.onClick.AddListener(OnDeleteButtonClicked);
        // 只有当没有实例时，才赋值
        instance = this;
    }
    private void Start()
    {
        //从装备背包里加载装备
        LoadEquippedItemsFromJinSlot();
        //启动时刷新武器槽
        RefreshWeaponSlots();
        // 初始隐藏按钮
      //  SetButtonsActive(false, false, false);
    }
    private void LoadEquippedItemsFromJinSlot()
    {
        if (jinSlot == null) return;
        // 索引0=近战武器槽，1=远程武器槽（与JinSlot.equippedItems对应）
        currentMeleeWeapon = jinSlot.itemList[0];
        currentRangedWeapon = jinSlot.itemList[1];
        currentfangyuWeapon = jinSlot.itemList[2];
    }
    // 设置按钮的激活状态
    private void SetButtonsActive(bool equip, bool unequip, bool delete)
    {
        equipButton.interactable = equip;
        unequipButton.interactable = unequip;
        deleteButton.interactable = delete;
    }
    private void OnEnable()
    {
        RefreshItem();
        instance.itemInfromation.text = "";      //初始化描述框

        // 刷新武器槽显示
        RefreshWeaponSlots();
      //  SetButtonsActive(false, false, false);
    }
    public void SelectSlot(Slot slot)
    {
        

        // 设置新的选中格子
        currentSelectedSlot = slot;

        // 更新按钮可用状态
        if (currentSelectedSlot == null || currentSelectedSlot.slotItem == null)
        {
            // 选中空格子，所有按钮不可用
            SetButtonsActive(false, false, false);
            return;
        }
        switch (currentSelectedSlot.slotType)
        {
            case Slot.SlotType.BagSlot:
                // 背包格子：装备按钮（仅武器可用）、删除按钮可用，卸下按钮不可用
                bool canEquip = currentSelectedSlot.slotItem.itemType == Item.ItemType.MeleeWeapon ||
                                currentSelectedSlot.slotItem.itemType == Item.ItemType.RangedWeapon;
                SetButtonsActive(canEquip, false, true);
                break;

            case Slot.SlotType.MeleeWeaponSlot:
            case Slot.SlotType.RangedWeaponSlot:
                // 武器槽：卸下按钮可用，其他按钮不可用
                SetButtonsActive(false, true, false);
                break;
        }
    }
    public static void UpdateItemInfo(string itemDescription)
    {        //更新描述
        instance.itemInfromation.text = itemDescription;
    }

    // 刷新武器槽的显示
    public void RefreshWeaponSlots()
    {
        // 刷新近战武器槽
        meleeWeaponSlot.SetupSlot(currentMeleeWeapon);
        // 刷新远程武器槽
        rangedWeaponSlot.SetupSlot(currentRangedWeapon);
        fangyuSlot.SetupSlot(currentfangyuWeapon);
    }

    //public static void CreateNewItem(Item item)
    //{   //把Item（物品）放进slot（格子）
    //    Slot newItem = Instantiate(instance.slotPrefab, instance.slotGrid.transform.position, Quaternion.identity);
    //    //把生成的这个物体和他的父级挂在一起
    //    newItem .gameObject .transform.SetParent (instance.slotGrid.transform);
    //    //再把这个物体的基本信息传过来（或者说放到格子上）
    //    newItem.slotItem = item;
    //    newItem .slotImage.sprite=item.itemImage;
    //    newItem .slotNum.text=item.itemHeld.ToString();
    //}
    public static void RefreshItem()
    {      //列表中的数量加1，把格子里的所有东西都删除，然后重新添加就好了
        for (int i = 0; i < instance.slotGrid.transform.childCount; i++)
        {
            if (instance.slotGrid.transform.childCount == 0)
                break;
            Destroy(instance.slotGrid.transform.GetChild(i).gameObject);
            instance.slots.Clear();     //把背包列表清空
        }
        for (int i = 0; i < instance.myBag.itemList.Count; i++)
        {
            //  CreateNewItem(instance.myBag.itemList[i]);
            instance.slots.Add(Instantiate(instance.emptySlot));   //生成18个格子
            instance.slots[i].transform.SetParent(instance.slotGrid.transform);
            instance.slots[i].GetComponent<Slot>().slotID = i;
            //把背包列表里的物品给到格子
            instance.slots[i].GetComponent<Slot>().SetupSlot(instance.myBag.itemList[i]);
        }
        instance.RefreshWeaponSlots();//刷新武器格子
    }


    public static bool EquipWeaponToSlot(Item item, Slot targetSlot)
    {
        if (item == null)
        {
            
            return false;
        }
        // 判断物品类型和槽位类型是否匹配
        if (item.itemType == Item.ItemType.MeleeWeapon && targetSlot.slotType == Slot.SlotType.MeleeWeaponSlot)
        {
            // 先把原有武器放回背包（如果有）
            if (instance.currentMeleeWeapon != null)
            {
                AddItemToBag(instance.currentMeleeWeapon);
            }
            // 装备新武器
            instance.currentMeleeWeapon = item;
            // 从背包移除该武器
            RemoveItemFromBag(item);
            instance.jinSlot.itemList[0] = item;  //同步到近战
            instance.OnWeaponChanged?.Invoke(instance.currentMeleeWeapon, instance.currentRangedWeapon,instance.currentfangyuWeapon);   //广播
            return true;
        }
        else if (item.itemType == Item.ItemType.RangedWeapon && targetSlot.slotType == Slot.SlotType.RangedWeaponSlot)
        {
            // 先把原有武器放回背包（如果有）
            if (instance.currentRangedWeapon != null)
            {
                AddItemToBag(instance.currentRangedWeapon);
            }
            // 装备新武器
            instance.currentRangedWeapon = item;
            // 从背包移除该武器
            RemoveItemFromBag(item);
            instance.jinSlot.itemList[1] = item;
            instance.OnWeaponChanged?.Invoke(instance.currentMeleeWeapon, instance.currentRangedWeapon, instance.currentfangyuWeapon);  //广播
            return true;
        }
        else if (item.itemType == Item.ItemType.Fangyu && targetSlot.slotType == Slot.SlotType.FangyuSlot)
        {
            // 先把原有武器放回背包（如果有）
            if (instance.currentfangyuWeapon != null)
            {
                AddItemToBag(instance.currentfangyuWeapon);
            }
            // 装备新武器
            instance.currentfangyuWeapon = item;
            // 从背包移除该武器
            RemoveItemFromBag(item);
            instance.jinSlot.itemList[2] = item;
            instance.OnWeaponChanged?.Invoke(instance.currentMeleeWeapon, instance.currentRangedWeapon, instance.currentfangyuWeapon);  //广播
            return true;
        }
        return false;
    }

    // 将物品添加到背包的辅助方法
    public static void AddItemToBag(Item item)
    {
        // 找背包的空位
        for (int i = 0; i < instance.myBag.itemList.Count; i++)
        {
            if (instance.myBag.itemList[i] == null)
            {
                instance.myBag.itemList[i] = item;
                return;
            }
        }
        // 没有空位则直接添加
        instance.myBag.itemList.Add(item);
    }

    // 从背包移除物品的辅助方法
    private static void RemoveItemFromBag(Item item)
    {
        for (int i = 0; i < instance.myBag.itemList.Count; i++)
        {
            if (instance.myBag.itemList[i] == item)
            {
                instance.myBag.itemList[i] = null;
                break;
            }
        }
    }

    //卸下武器时同步从JinSlot移除
    public void UnequipWeapon(Slot.SlotType slotType)
    {
        if (jinSlot == null) return;

        if (slotType == Slot.SlotType.MeleeWeaponSlot)
        {
            if (currentMeleeWeapon != null)
            {
                AddItemToBag(currentMeleeWeapon);
                currentMeleeWeapon = null;
                jinSlot.itemList[0] = null; // 同步JinSlot
                OnWeaponChanged?.Invoke(currentMeleeWeapon, currentRangedWeapon,currentfangyuWeapon);
            }
        }
        else if (slotType == Slot.SlotType.RangedWeaponSlot)
        {
            if (currentRangedWeapon != null)
            {
                AddItemToBag(currentRangedWeapon);
                currentRangedWeapon = null;
                jinSlot.itemList[1] = null; // 同步JinSlot
                OnWeaponChanged?.Invoke(currentMeleeWeapon, currentRangedWeapon, currentfangyuWeapon);
            }
        }
        else if (slotType == Slot.SlotType.FangyuSlot)
        {
            if (currentfangyuWeapon != null)
            {
                AddItemToBag(currentfangyuWeapon);
                currentfangyuWeapon = null;
                jinSlot.itemList[2] = null; // 同步JinSlot
                OnWeaponChanged?.Invoke(currentMeleeWeapon, currentRangedWeapon, currentfangyuWeapon);
            }
        }
        RefreshWeaponSlots();
    }
}