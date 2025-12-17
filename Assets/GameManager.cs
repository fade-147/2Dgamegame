using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    //单例模式
    public static GameManager Instance { get; private set; }
    private int coinCount;


    // 公开的只读属性：供外部获取金币数量，不能直接修改
    public int CoinCount => coinCount;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }else if(Instance != this)
         {
            Destroy (gameObject);
        }
        DontDestroyOnLoad (gameObject);      //加载新场景时高度游戏引擎，不要销毁该对象
    }
    //增加金币
    public void AddCoins(int value)
    {
        Debug.Log(value);
        coinCount+=value;
        UICoinCountText.UpateText(coinCount);     //更新金币文本UI
    }
    //减少金币
    public void RemoveCoins(int value)
    {
        coinCount-=value;
        UICoinCountText.UpateText(coinCount);     //更新金币文本UI
    }
}