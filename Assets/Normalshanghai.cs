using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Normalshanghai : MonoBehaviour
{
    [Header("核心引用")]
    public GameObject baojian1; // 角色物体（拖拽角色到这里）
    public GameObject baojian2; // 角色物体（拖拽角色到这里）
    public GameObject baojian3; // 角色物体（拖拽角色到这里）
    public int attackPerLevel = 3; // 每级增加的攻击力（固定3）

    private TMP_Text talentLevelText; // 自身的等级文本组件
    private Attack playerAttack1; // 角色的Attack脚本
    private Attack playerAttack2; // 角色的Attack脚本
    private Attack playerAttack3; // 角色的Attack脚本

    private void Awake()
    {
        talentLevelText = GetComponent<TMP_Text>();
        playerAttack1 = baojian1.GetComponent<Attack>();
        playerAttack2 = baojian2.GetComponent<Attack>();
        playerAttack3 = baojian3.GetComponent<Attack>();
    }

    private void Start()
    {
        // 游戏启动时先同步一次攻击力
        UpdateAttackDamage();
    }
    private void Update()
    {
        UpdateAttackDamage();
    }

    // 核心：计算并更新攻击力（基础damage + 等级×3）
    public void UpdateAttackDamage()
    {
        // 空值保护
        if (playerAttack1 == null || talentLevelText == null|| playerAttack2 == null|| playerAttack3 == null) return;

        //读取等级文本的数字（纯数字文本直接解析）
        int talentLevel = 0;
        int.TryParse(talentLevelText.text, out talentLevel);
        playerAttack1.damage = 10 + (talentLevel * attackPerLevel);
        playerAttack2.damage = 15 + (talentLevel * attackPerLevel);
        playerAttack3.damage = 15 + (talentLevel * attackPerLevel);
    }
}
