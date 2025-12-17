using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    public PropPrefab[] propPrefabs;  //存储不同道具的预制体

    //开始生成掉落道具
    public void DropItems()
    {
        foreach (var propPrefab in propPrefabs)
        {
            if(Random.Range(0f,100f)<=propPrefab.dropPercentage)
            {
                Instantiate (propPrefab.prefab ,transform.position, Quaternion.identity);
            }
        }
    }
}

[System.Serializable]  //没有mono behaviour，但想在编译器上显示
public class PropPrefab
{
    public GameObject prefab;
    [Range(0f,100f)]public float dropPercentage;
}

