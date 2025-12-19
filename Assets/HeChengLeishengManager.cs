using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeChengLeishengManager : MonoBehaviour
{
    // 合成配置（在Inspector中赋值）
    [Header("合成原料配置")]
    public Item material1;       
    public Image material1Image; 
    public Item material2;       
    public Image material2Image; 

    [Header("合成成品配置")]
    public Item HeCheng1;     // 合成
    public Button HeChengButton;   // 合成按钮
    public GameObject NormalText;
    public GameObject OKText;

    // 亮度控制（暗/正常）
    private Color normalColor = Color.white;
    private Color darkColor = new Color(0.4f, 0.4f, 0.4f, 1f); // 灰色（亮度降低）

    private void OnEnable()
    {
        // 面板激活时初始化状态
        UpdateCraftState();
        OKText.SetActive(false);
        NormalText.SetActive(true);
        // 绑定合成按钮事件
        HeChengButton.onClick.AddListener(OnCraftButtonClicked);
    }

    private void OnDisable()
    {
        // 面板禁用时解绑事件，避免重复绑定
        HeChengButton.onClick.RemoveListener(OnCraftButtonClicked);
    }

    private void Update()
    {
        // 实时检测原料状态（也可以改为只在背包刷新后检测，优化性能）
        UpdateCraftState();
    }


    // 更新合成状态：检测原料是否存在，更新Image亮度和按钮状态
    private void UpdateCraftState()
    {
        if (InventoryManager.instance == null || InventoryManager.instance.myBag == null)
        {
            return;
        }
        // 检测背包中是否存在原料1/原料2
        bool hasMaterial1 = CheckItemInBag(material1);
        bool hasMaterial2 = CheckItemInBag(material2);
        // 更新原料Image亮度
        material1Image.color = hasMaterial1 ? normalColor : darkColor;
        material2Image.color = hasMaterial2 ? normalColor : darkColor;
        // 只有两个原料都存在时，合成按钮才可点击
        HeChengButton.interactable = hasMaterial1 && hasMaterial2;
    }


    // 检测背包中是否存在指定Item
    private bool CheckItemInBag(Item targetItem)
    {
        if (targetItem == null) return false;

        List<Item> bagItems = InventoryManager.instance.myBag.itemList;
        for (int i = 0; i < bagItems.Count; i++)
        {
            if (bagItems[i] != null && bagItems[i].itemName == targetItem.itemName)
            {
                return true;
            }
        }
        return false;
    }


    // 合成按钮点击逻辑
    private void OnCraftButtonClicked()
    {
        bool hasMaterial1 = CheckItemInBag(material1);
        bool hasMaterial2 = CheckItemInBag(material2);
        if (!hasMaterial1 || !hasMaterial2)
        {
            return;
        }
        //从背包中删除两个原料
        RemoveItemFromBag(material1);
        RemoveItemFromBag(material2);
        // 向背包中添加合成品
        InventoryManager.AddItemToBag(HeCheng1);
        // 刷新背包UI和合成面板状态
        InventoryManager.RefreshItem();
        UpdateCraftState();
        StartCoroutine(ToggleOKHeChengCoroutine());
    }

    // 从背包中删除指定Item（按itemName匹配）
    private void RemoveItemFromBag(Item targetItem)
    {
        if (targetItem == null) return;

        List<Item> bagItems = InventoryManager.instance.myBag.itemList;
        for (int i = 0; i < bagItems.Count; i++)
        {
            if (bagItems[i] != null && bagItems[i].itemName == targetItem.itemName)
            {
                bagItems[i] = null; // 置空
                break; 
            }
        }
    }
    private IEnumerator ToggleOKHeChengCoroutine()
    {
        OKText.SetActive(true);
        NormalText.SetActive(false);
        yield return new WaitForSeconds(1f);
        OKText.SetActive(false);
        NormalText.SetActive(true);
    }
}
