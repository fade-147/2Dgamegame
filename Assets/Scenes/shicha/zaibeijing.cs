using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEngine.RuleTile.TilingRuleOutput;

public class zaibeijing : MonoBehaviour
{
    [Header("无限地图")]
    public GameObject mainCamera;         // 主摄像机对象
    public float mapWidth;                // 地图宽度
    public int mapNums;                   // 地图重复的次数
    private float totalWidth;              // 总地图宽度
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");        // "MainCamera"8斗治井鼻画
        mapWidth = GetComponent<SpriteRenderer>().sprite.bounds.size.x;       // 通过SpriteRenderer获得图像宽度
        totalWidth = mapWidth * mapNums;                            // 计算总地图宽度
    }

    void Update()
    {
        Vector3 tempPosition = transform.position;        // 获取当前位置
        if (mainCamera.transform.position.x > transform.position.x + totalWidth / 2)
        {
            tempPosition.x += totalWidth;              // 将地图向右平移一个完整的地图宽度
            transform.position = tempPosition;             // 更新位置
        }
        else if (mainCamera.transform.position.x < transform.position.x - totalWidth / 2)
        {
            tempPosition.x -= totalWidth;                // 将地图向左平移一个完整的地图宽度
            transform.position = tempPosition;           // 更新位置
        }
    }
}
