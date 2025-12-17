using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChongwuManager : MonoBehaviour
{
    public GameObject Chongwu;
    public GameObject ChongwuUI;
    public VoidEventSO GetChongBlueEvent;

    private void OnEnable()
    {
        if (GetChongBlueEvent != null)
        {
            // 订阅事件触发时，执行OnBossKilled方法
            GetChongBlueEvent.OnEventRaised += OnBossKilled;
        }
    }

    private void OnDisable()
    {
        if (GetChongBlueEvent != null)
        {
            GetChongBlueEvent.OnEventRaised -= OnBossKilled;
        }
    }

    private void OnBossKilled()
    {

        Chongwu.SetActive(true);
        ChongwuUI.SetActive(true);

        Destroy(this);

    }
}
