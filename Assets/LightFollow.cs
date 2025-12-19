using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFollow : MonoBehaviour
{
    public float maxIntensity = 3f;  
    public float minIntensity = 0f;   
    public float maxDistance = 6f;   
    public Light2D targetLight;
    private Transform playerTransform;


    void Update()
    {
        if (GameObject.FindGameObjectWithTag("Player")!=null)
        {
            // 通过标签获取玩家Transform
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }
        if (playerTransform != null && targetLight != null)
        {
            // 计算光源与玩家的距离
            float distance = Vector3.Distance(transform.position, playerTransform.position);

            if (distance <= maxDistance)
            {
                // 距离越近，亮度越高
                float intensity = Mathf.Lerp(maxIntensity, minIntensity, distance / maxDistance);
                targetLight.intensity = intensity;
            }
            else
            {
                // 超出距离，亮度设为最小值
                targetLight.intensity = minIntensity;
            }
        }
    }
}
