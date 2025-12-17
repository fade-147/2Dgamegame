using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    //// 拖入你的枪的ItemData（Qiang）
    //public ItemData itemData;

    //// 玩家碰撞到这个物体时触发
    //private void OnTriggerEnter(Collider other)
    //{
    //    // 检测是否是玩家（确保你的玩家标签是“Player”）
    //    if (other.CompareTag("Player"))
    //    {
    //        // 尝试添加到背包
    //        bool isAdded = InventoryManager.instance.AddItem(itemData);

    //        if (isAdded)
    //        {
    //            Debug.Log($"拾取了【{itemData.itemName}】！");
    //            // 拾取后销毁场景中的枪实体
    //            Destroy(gameObject);
    //        }
    //        else
    //        {
    //            Debug.LogWarning("背包已满，无法拾取！");
    //        }
    //    }
    //}
}
