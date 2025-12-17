using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class huodejinshen : MonoBehaviour
{
    public Button skillsBtn;
    public Image skillImage;
    public int upgradeCost = 3; // 解锁消耗3金币
    public Button nextButton1;
    public Button nextButton2;
    public wudijinshen jinshenjinshen;     //用来控制获得新技能

    private void Awake()
    {
        skillsBtn.onClick.AddListener(PriseSkillPanel);
        Color tempColor = skillImage.color;
        tempColor.a = 0.3f;
        skillImage.color = tempColor;
        skillsBtn.interactable = false;
    }

    private void PriseSkillPanel()
    {
        int currentCoins = GameManager.Instance.CoinCount;
        if (currentCoins < upgradeCost)
        {
            return; // 金币不够，直接退出
        }

        // 消耗金币
        GameManager.Instance.RemoveCoins(upgradeCost);

        // 满透明度（alpha设为1=完全不透明）
        Color unlockColor = skillImage.color;
        unlockColor.a = 1f; // alpha范围0（全透）~1（全不透）
        skillImage.color = unlockColor;

        if (nextButton1 != null)
            nextButton1.interactable = true;

        if (nextButton2 != null)
            nextButton2.interactable = true;

        jinshenjinshen.jiesuo = true;     //获得新技能
        //禁用按钮，防止重复点击消耗金币
        skillsBtn.interactable = false;
    }
}