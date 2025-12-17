using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIMangager : MonoBehaviour
{
    public PlayerStatBar playerStatBar;

    [Header("事件监听")]
    public CharacterEvent healthEvent;
    public SceneLoadEvent unloadedSceneEvent;
    public VoidEventSO loadDataEvent;
    public VoidEventSO gameOverEvent;
    public VoidEventSO backToMenuEvent;

    public FloatEventSO syncVolumeEvent;

    [Header("广播")]
    public VoidEventSO pauseEvent;

    [Header("组件")]
    public GameObject gameOverPanal;
    public GameObject restartBtn;
    public Slider volumeSlider;

    public Button settingsBtn;
    public GameObject pausePanel;
    public Button TianfuBtn;
    public GameObject TianfuPanel;
    public Button WuqiBtn;
    public GameObject WuqiPanel;
    public Button Beibao;
    public GameObject BeibaoPanel;

    private void Awake()
    {
        settingsBtn.onClick.AddListener(TogglePausePanel);   //这下每次点击齿轮按钮都会执行这个命令
        TianfuBtn.onClick.AddListener(ToggleTianfuPanel);
        WuqiBtn.onClick.AddListener(ToggleWuqiPanel);
        Beibao.onClick.AddListener(ToggleBeibaoPanel);
    }
    public void OnEnable()
    {
        healthEvent.OnEventRaised += OnHealthEvent;
        unloadedSceneEvent.loadRequestEvent  += OnUnLoadedSceneEvent;
        loadDataEvent.OnEventRaised += OnLoadDataEvent;
        gameOverEvent.OnEventRaised += OnGameOverEvent;
        backToMenuEvent.OnEventRaised += OnLoadDataEvent;

        syncVolumeEvent.OnEventRaised += OnSyncVolumeEvent;      //接收广播，把当前的mixer传给滑动条
    }


    public void OnDisable()
    {
        healthEvent.OnEventRaised -= OnHealthEvent;
        unloadedSceneEvent.loadRequestEvent  -= OnUnLoadedSceneEvent;
        loadDataEvent.OnEventRaised -= OnLoadDataEvent;
        gameOverEvent.OnEventRaised -= OnGameOverEvent;
        backToMenuEvent.OnEventRaised -= OnLoadDataEvent;

        syncVolumeEvent.OnEventRaised -= OnSyncVolumeEvent;
    }

    private void OnSyncVolumeEvent(float amount)
    {
        volumeSlider.value = (amount + 80) / 100;
    }

    public void TogglePausePanel()
    {
        if (TianfuPanel.activeInHierarchy)          //如果其他两个画布存在就关闭
        {
            TianfuPanel.SetActive(false);
        }

        if (WuqiPanel.activeInHierarchy)
        {
            WuqiPanel.SetActive(false);
        }
        if (BeibaoPanel.activeInHierarchy)
        {
            BeibaoPanel.SetActive(false);
        }

        if (pausePanel.activeInHierarchy)   //菜单如果已经激活了，那么再点一下就关闭
        {
            pausePanel .SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            pauseEvent.RaiseEvent();
            pausePanel .SetActive(true);
            Time.timeScale = 0;
        }
    }
    private void ToggleTianfuPanel()
    {
        if (pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(false);
        }
        if (WuqiPanel.activeInHierarchy)
        {
            WuqiPanel.SetActive(false);
        }
        if (BeibaoPanel.activeInHierarchy)
        {
            BeibaoPanel.SetActive(false);
        }

        if (TianfuPanel.activeInHierarchy)   //技能菜单如果已经激活了，那么再点一下就关闭
        {
            TianfuPanel.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
         //   pauseEvent.RaiseEvent();
            TianfuPanel.SetActive(true);
            Time.timeScale = 0;
        }
    }
    private void ToggleWuqiPanel()
    {
        if (TianfuPanel.activeInHierarchy)
        {
            TianfuPanel.SetActive(false);
        }
        if (pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(false);
        }
        if (BeibaoPanel.activeInHierarchy)
        {
            BeibaoPanel.SetActive(false);
        }

        if (WuqiPanel.activeInHierarchy)   //技能菜单如果已经激活了，那么再点一下就关闭
        {
            WuqiPanel.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
         //   pauseEvent.RaiseEvent();
            WuqiPanel.SetActive(true);
            Time.timeScale = 0;
        }
    }
    private void ToggleBeibaoPanel()
    {
        if (TianfuPanel.activeInHierarchy)
        {
            TianfuPanel.SetActive(false);
        }
        if (pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(false);
        }
        if (WuqiPanel.activeInHierarchy)
        {
            WuqiPanel.SetActive(false);
        }

        if (BeibaoPanel.activeInHierarchy)   //背包如果已经激活了，那么再点一下就关闭
        {
            BeibaoPanel.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
         //   pauseEvent.RaiseEvent();
            BeibaoPanel.SetActive(true);
            Time.timeScale = 0;
        }
    }
    private void OnGameOverEvent()
    {
        gameOverPanal.SetActive(true);
        EventSystem.current.SetSelectedGameObject(restartBtn);
    }
    private void OnLoadDataEvent()
    {
        gameOverPanal.SetActive(false);
    }

    private void OnUnLoadedSceneEvent(GameSceneSO sceneToLoad, Vector3 arg1, bool arg2)
    {
        var isMenu = sceneToLoad.sceneType == SceneType.Menu;
         playerStatBar.gameObject.SetActive(!isMenu);
    }

    private void OnHealthEvent(Character character)
    {
        var persentage = character.currentHealth / character.maxHealth;
        playerStatBar .OnHealthChange (persentage);
    }
}