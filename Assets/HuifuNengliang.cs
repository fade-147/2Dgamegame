using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HuifuNengliang : MonoBehaviour
{
    [Header("核心引用")]
    public GameObject Chongci; // 角色物体（拖拽角色到这里）
    public int attackPerLevel = 3; // 每级增加的攻击力（固定3）

    private TMP_Text talentLevelText; // 自身的等级文本组件
    private Chongci chongciPower; // 角色的Attack脚本

    private void Awake()
    {
        talentLevelText = GetComponent<TMP_Text>();
        chongciPower = Chongci.GetComponent<Chongci>();
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

    // 核心：计算并更新攻击力
    private void UpdateAttackDamage()
    {
        // 空值保护
        if (chongciPower == null || talentLevelText == null) return;

        //读取等级文本的数字（纯数字文本直接解析）
        int talentLevel = 0;
        int.TryParse(talentLevelText.text, out talentLevel);
        chongciPower.HuifuPower= 4+(talentLevel * attackPerLevel);
    }
}
