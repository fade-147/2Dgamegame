using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using UnityEngine.UI;

public class TianfuManager : MonoBehaviour
{
    public Button skillsBtn;
    public TMP_Text talentLevelText;
    public int upgradeCost = 3; // 升级一次消耗金币的数量
    private int currentTalentLevel = 0; // 记录当前等级的变量
    public Normalshanghai updateshanghai;
    private void Awake()
    {
        skillsBtn.onClick.AddListener(PriseSkillPanel);
    }
    private void PriseSkillPanel()
    {
        int currentCoins = GameManager.Instance.CoinCount;
        if (currentCoins < upgradeCost)
        {
            return;
        }
        GameManager.Instance.RemoveCoins(upgradeCost);
        //等级+1
        currentTalentLevel++;

        // 更新文本显示（这一步就是让文本变成新等级）
        UpdateLevelText();
        if(updateshanghai  != null)
        {
            updateshanghai.UpdateAttackDamage();
        }

    }

    // 专门负责更新等级文本的方法
    private void UpdateLevelText()
    {
        // 把等级变量转成字符串，赋值给TMP文本的text属性
        talentLevelText.text = currentTalentLevel.ToString();
    }
}
