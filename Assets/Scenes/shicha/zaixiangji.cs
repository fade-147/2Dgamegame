using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEngine.RuleTile.TilingRuleOutput;
public class zaixiangji : MonoBehaviour
{
    public Transform target;             // 玩家的位置
    public Transform farBackground, middleBackground;    //远的背景和中间背景的位置
    private Vector2 lastPos;             //  最后一次的相机位置
    public bool finishFind=false;
    void Start()
    {
        lastPos = transform.position; //记录相机的初始位置
        
    }

    // 通过标签查找游戏对象并获取其 Transform
    Transform FindTransformByTag(string tagName)
    {
        GameObject foundObj = GameObject.FindGameObjectWithTag(tagName);
        if (foundObj != null)
        {
            finishFind = true;
            return foundObj.transform;
        }
        else
        {
            finishFind = false;
            return foundObj.transform;
        }

       //     Debug.LogError($"未找到标签为 '{tagName}' 的游戏对象");
      //  return null;
    }

    // 使用示例
    public void GetBackgroundTransformByTag()
    {
        // 确保已在标签管理器中创建"Background"标签
        farBackground  = FindTransformByTag("farBackground");
        middleBackground = FindTransformByTag("middleBackground");


    }

    void Update()
    {
        if (GameObject.FindGameObjectWithTag("farBackground") != null)       //防止空引用
        {
            GetBackgroundTransformByTag();
        }
        else
        {
            return;                        //防止在场景切换时持续查找而报错
        }

        if (!finishFind) return;
        // 将相机的位置设置为玩家的位置，但限制在一定的垂直范围内
        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);

        // 计算相机在上一帧和当前帧之间移动的距离
        Vector2 amountToMove = new Vector2(transform.position.x - lastPos.x, transform.position.y - lastPos.y);

        //根据相机移动的距离，移动远背景和中间背景的位置
        farBackground.position += new Vector3(amountToMove.x, amountToMove.y, 0f);
        middleBackground.position += new Vector3(amountToMove.x * 0.5f, amountToMove.y * 0.5f,0f);

        lastPos = transform.position;             // 更新最后一次的相机位置
     }
}