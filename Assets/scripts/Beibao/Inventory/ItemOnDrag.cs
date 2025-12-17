using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemOnDrag : MonoBehaviour,IBeginDragHandler,IDragHandler, IEndDragHandler     //增加三个接口
{
    //分别是开始拖拽（点击），拽的过程中，和结束拖拽
    public Transform originalParent;
    public Inventory myBag;
    private int currentItemID;   //当前物品ID（所在格子的序号）

    private Item currentItem;    // 当前拖拽的物品
    private bool isFromWeaponSlot = false;  //是否正在拖拽
    public void OnBeginDrag(PointerEventData eventData)
    {
        
        originalParent = transform.parent;    //先记录好他的父级，这样在拖拽结束后可以换格子或者和其他格子的物品交换
        // currentItemID=originalParent.GetComponent<Slot>().slotID;    //这样就获得了物体的ID，物体的ID就是背包的ID

        Slot originalSlot = originalParent.GetComponent<Slot>();// 新增：判断是否从武器槽拖拽
        if (originalSlot == null)
        {
            // 原父级没有Slot组件，直接终止拖拽
            Debug.LogWarning("拖拽的物体父级没有Slot组件！");
            return;
        }
        isFromWeaponSlot = (originalSlot.slotType == Slot.SlotType.MeleeWeaponSlot
                          || originalSlot.slotType == Slot.SlotType.RangedWeaponSlot|| originalSlot.slotType == Slot.SlotType.FangyuSlot);

        if (!isFromWeaponSlot)
        {
            currentItemID = originalParent.GetComponent<Slot>().slotID;
        }
        transform.SetParent(transform.parent.parent);   //先让他成为他父级的父级，（也能防止他作为子级而被其他格子挡住）
        transform.position = eventData.position;    //点击鼠标后，让该物体的位置等于鼠标的位置

        // 新增：获取当前拖拽的物品
        currentItem = originalSlot.slotItem; // 统一从原槽位获取物品
        //这个是鼠标射线，鼠标向屏幕发射一条射线，可以知道鼠标说在位置的最上层物品
        //但是一但拖拽物品了，就会挡住射线，无法知道鼠标所在哪个格子上面，所以先把这个功能关闭，拖拽结束再打开
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    //public void OnEndDrag(PointerEventData eventData)
    //{
    //    if (eventData.pointerCurrentRaycast.gameObject != null)
    //    {
    //        if (eventData.pointerCurrentRaycast.gameObject.name == "Item Image")  //说明格子里有东西，需要交换
    //        {
    //            transform.SetParent(eventData.pointerCurrentRaycast.gameObject.transform.parent.parent);
    //            transform.position = eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.position;

    //            var temp = myBag.itemList[currentItemID];
    //            myBag.itemList[currentItemID] = myBag.itemList[eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<Slot>().slotID];
    //            myBag.itemList[eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<Slot>().slotID] = temp;

    //            eventData.pointerCurrentRaycast.gameObject.transform.parent.position = originalParent.position;
    //            eventData.pointerCurrentRaycast.gameObject.transform.parent.SetParent(originalParent);
    //            GetComponent<CanvasGroup>().blocksRaycasts = true;
    //            return;
    //        }

    //        if (eventData.pointerCurrentRaycast.gameObject.name == "slot(Clone)")
    //        {
    //            //格子里没东西，直接“放进去”
    //            transform.SetParent(eventData.pointerCurrentRaycast.gameObject.transform);
    //            transform.position = eventData.pointerCurrentRaycast.gameObject.transform.position;
    //            //itemList的物体存储位置改变
    //            myBag.itemList[eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<Slot>().slotID] = myBag.itemList[currentItemID];
    //            if (eventData.pointerCurrentRaycast.gameObject.GetComponent<Slot>().slotID != currentItemID)
    //                myBag.itemList[currentItemID] = null;

    //            GetComponent<CanvasGroup>().blocksRaycasts = true;
    //            return;
    //        }
    //    }

    //  //其他任何位置都归位
    //    transform.SetParent(originalParent);
    //    transform .position = originalParent.position;
    //    GetComponent<CanvasGroup>().blocksRaycasts = true;
    //}

    public void OnEndDrag(PointerEventData eventData)
    {
        // 重置射线阻挡
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        // 判断拖拽的目标对象
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            GameObject targetObj = eventData.pointerCurrentRaycast.gameObject;
            Slot targetSlot = null;

            // 情况1：目标是武器槽（直接获取Slot组件）
            if (targetObj.GetComponent<Slot>() != null)
            {
                targetSlot = targetObj.GetComponent<Slot>();
            }
            // 情况2：目标是物品图片（获取父级的Slot组件）
            else if (targetObj.name == "Item Image")
            {
                targetSlot = targetObj.transform.parent.parent.GetComponent<Slot>();
            }
            // 情况3：目标是背包格子（Clone）
            else if (targetObj.name == "slot(Clone)")
            {
                targetSlot = targetObj.GetComponent<Slot>();
            }

            // 新增：处理【从武器槽拖出到背包】的逻辑
            if (isFromWeaponSlot)
            {
                // 拖到背包槽 → 放回背包
                if (targetSlot != null && targetSlot.slotType == Slot.SlotType.BagSlot)
                {
                    // 记录原武器框的信息调用UnequipWeapon
                    //从装备背包里移除该物品
                    Slot originalSlot = originalParent.GetComponent<Slot>();
                    InventoryManager.instance.UnequipWeapon(originalSlot.slotType);

                    //记录原武器框的信息（位置、父物体、类型）
                    Transform originalParentTrans = originalParent.parent; // 原武器框的父物体（比如UI面板）
                    Vector3 originalPosition = originalParent.position;     // 原武器框的位置
                    Quaternion originalRotation = originalParent.rotation;  // 原武器框的旋转
                    Slot.SlotType originalSlotType = originalSlot.slotType;  // 原武器框类型（近战/远程）

                    // 3. 清空原武器槽的引用（ InventoryManager 中的当前武器）
                    if (originalSlotType == Slot.SlotType.MeleeWeaponSlot)
                    {
                        InventoryManager.instance.currentMeleeWeapon = null;
                    }
                    else if (originalSlotType == Slot.SlotType.RangedWeaponSlot)
                    {
                        InventoryManager.instance.currentRangedWeapon = null;
                    }
                    else if (originalSlotType == Slot.SlotType.FangyuSlot)
                    {
                        InventoryManager.instance.currentfangyuWeapon = null;
                    }

                    // 4. 销毁原武器框
                    Destroy(originalParent.gameObject);

                    // 5. 根据原类型生成新的武器框预制体
                    GameObject newWeaponSlot = null;
                    if (originalSlotType == Slot.SlotType.MeleeWeaponSlot)
                    {
                        newWeaponSlot = Instantiate(InventoryManager.instance.meleeWeaponSlotPrefab, originalParentTrans);
                        // 更新 InventoryManager 中的近战武器槽引用
                        InventoryManager.instance.meleeWeaponSlot = newWeaponSlot.GetComponent<Slot>();
                    }
                    else if (originalSlotType == Slot.SlotType.RangedWeaponSlot)
                    {
                        newWeaponSlot = Instantiate(InventoryManager.instance.rangedWeaponSlotPrefab, originalParentTrans);
                        // 更新 InventoryManager 中的远程武器槽引用
                        InventoryManager.instance.rangedWeaponSlot = newWeaponSlot.GetComponent<Slot>();
                    }
                    else if (originalSlotType == Slot.SlotType.FangyuSlot)
                    {
                        newWeaponSlot = Instantiate(InventoryManager.instance.fangyuPrefab, originalParentTrans);
                        // 更新 InventoryManager 中的远程武器槽引用
                        InventoryManager.instance.fangyuSlot = newWeaponSlot.GetComponent<Slot>();
                    }

                    // 6. 设置新武器框的位置和旋转（与原武器框一致）
                    if (newWeaponSlot != null)
                    {
                        newWeaponSlot.transform.position = originalPosition;
                        newWeaponSlot.transform.rotation = originalRotation;
                        newWeaponSlot.transform.localScale = originalParent.lossyScale; // 保持相同缩放
                    }


                    InventoryManager.RefreshItem();
                    InventoryManager.instance.RefreshWeaponSlots(); // 刷新新武器框的显示（空槽状态）

                    Destroy(gameObject); // 销毁拖拽的图标
                    return;
                }
                // 拖到非背包位置 → 放回武器槽
                else
                {
                    transform.SetParent(originalParent);
                    transform.position = originalParent.position;
                    return;
                }
            }


            // 原逻辑：处理【从背包拖到武器槽/背包槽】的逻辑
            if (targetSlot != null)
            {
                // 拖到武器槽 → 装备武器
                if (targetSlot.slotType == Slot.SlotType.MeleeWeaponSlot || targetSlot.slotType == Slot.SlotType.RangedWeaponSlot || targetSlot.slotType == Slot.SlotType.FangyuSlot)
                {
                    if (InventoryManager.EquipWeaponToSlot(currentItem, targetSlot))
                    {
                        InventoryManager.RefreshItem();
                        Destroy(gameObject);
                        return;
                    }
                    else
                    {
                        transform.SetParent(originalParent);
                        transform.position = originalParent.position;
                        return;
                    }
                }

                // 拖到背包槽 → 交换/放入物品
                if (targetSlot.slotType == Slot.SlotType.BagSlot)
                {
                    if (targetSlot.slotItem != null)
                    {
                        // 交换物品
                        var temp = myBag.itemList[currentItemID];
                        myBag.itemList[currentItemID] = myBag.itemList[targetSlot.slotID];
                        myBag.itemList[targetSlot.slotID] = temp;
                    }
                    else
                    {
                        // 放入空槽
                        myBag.itemList[targetSlot.slotID] = myBag.itemList[currentItemID];
                        myBag.itemList[currentItemID] = null;
                    }
                    InventoryManager.RefreshItem();
                    return;
                }
            }
        }

        // 其他情况 → 物品归位
        transform.SetParent(originalParent);
        transform.position = originalParent.position;
    }
}
