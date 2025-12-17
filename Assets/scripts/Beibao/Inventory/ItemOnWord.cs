using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOnWord : MonoBehaviour
{
    public Item thisItem;       //要知道该物体是个什么
    public Inventory playerInventory;     //把这个物体放进哪个背包
 
   

    public void AddNewItem()
    {     //拾取后判断，如果原来的列表（背包）没有这个物体，那么就加入这个物体
        if (!playerInventory.itemList.Contains(thisItem))
        {
            //放进背包列表中
            //playerInventory .itemList.Add(thisItem);

            //在背包panel的格子上显示他
            //InventoryManager.CreateNewItem (thisItem);
            for (int i = 0; i < playerInventory.itemList.Count; i++)
            {   //看看有没有空位，有空位就添加
                if (playerInventory.itemList[i] == null)
                {
                    playerInventory.itemList[i] = thisItem;
                    break;
                }
            }
        }
        else    //如果有那么就加1
        {
            thisItem.itemHeld += 1;
        }
        InventoryManager.RefreshItem();
    }
}
