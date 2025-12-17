using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour,ISaveable
{
    public Transform playerTrans;
    public Vector3 firstPosition;
    public Vector3 menuPosition;
    [Header("事件监听")]
    public SceneLoadEvent LoadEventSO;
    public VoidEventSO newGameEvent;
    public VoidEventSO backToMenuEvent;

    [Header("广播")]
    public VoidEventSO afterSceneLoadedEvent;
    public FadeEventSO fadeEvent;
    public SceneLoadEvent unloadedSceneEvent;

    [Header("场景")]
    public GameSceneSO firstLoadScene;
    public GameSceneSO menuScene;
    [SerializeField]private GameSceneSO currentLoadedScene;
    private GameSceneSO sceneToLoad;
    private Vector3 positionToGo;
    private bool fadeScreen;
    private bool isLoading;
    public float fadeDuration;

    private void Awake()
    {
        //Addressables.LoadSceneAsync(firstLoadScene.sceneReference, LoadSceneMode.Additive);
        //currentLoadedScene = firstLoadScene;
        //currentLoadedScene .sceneReference .LoadSceneAsync(LoadSceneMode.Additive);

    }

    private void Start()
    {
        //NewGame();
        LoadEventSO.RaiseLoadRequestEvent(menuScene, menuPosition, true);  //加载第一个场景是menu

    }
    private void OnEnable()
    {
        LoadEventSO.loadRequestEvent += OnLoadRequestEvent;
        newGameEvent.OnEventRaised += NewGame;
        backToMenuEvent.OnEventRaised += OnBackToMenuEvent;

        ISaveable saveable = this;
        saveable.RegisterSaveDate();
    }

    private void OnDisable()
    {
        LoadEventSO.loadRequestEvent -= OnLoadRequestEvent;
        newGameEvent.OnEventRaised -= NewGame;
        backToMenuEvent.OnEventRaised -= OnBackToMenuEvent;

        ISaveable saveable = this;
        saveable.UnregisterSaveDate();

    }

    private void OnBackToMenuEvent()
    {
        sceneToLoad = menuScene;
        LoadEventSO.RaiseLoadRequestEvent(sceneToLoad, menuPosition, true);
    }

    private void NewGame()
    {
        sceneToLoad = firstLoadScene;
        // OnLoadRequestEvent(sceneToLoad, firstPosition, true);
        LoadEventSO.RaiseLoadRequestEvent(sceneToLoad, firstPosition, true);
    }
    private void OnLoadRequestEvent(GameSceneSO LocationToLoad, Vector3 posToGo, bool fadeScreen)
    {
        if (isLoading) 
            return;

        isLoading = true;
        sceneToLoad = LocationToLoad;
        positionToGo= posToGo;
        this.fadeScreen = fadeScreen;
        if (currentLoadedScene != null)
        {
            StartCoroutine(UnLoadPreviousScene());
        }
        else
        {
            LoadNewScene();
        }
    }

    private IEnumerator UnLoadPreviousScene()
    {
        if(fadeScreen)
        {
            //变黑
            fadeEvent.FadeIn (fadeDuration);

        }

        yield return new WaitForSeconds(fadeDuration);  //等待场景完全变黑

        //广播时间调整血条显示
        unloadedSceneEvent.RaiseLoadRequestEvent(sceneToLoad,positionToGo ,true);

        yield return currentLoadedScene.sceneReference.UnLoadScene();

        //关闭人物
        playerTrans .gameObject.SetActive(false);
        //加载新场景
        LoadNewScene();
    }

    private void LoadNewScene()
    {
        var loadingOption = sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        loadingOption.Completed += OnLoadCompleted;
    }

    //场景加载完成后
    private void OnLoadCompleted(AsyncOperationHandle<SceneInstance> obj)
    {
        currentLoadedScene =sceneToLoad ;
        playerTrans.position = positionToGo;
        playerTrans .gameObject.SetActive(true);
        if(fadeScreen)
        {
            fadeEvent.FadeOut (fadeDuration);
        }
        isLoading = false;

        if(currentLoadedScene .sceneType!=SceneType.Menu)
        //场景加载完成后事件
        afterSceneLoadedEvent.RaiseEvent();
    }

    public DataDefinition GetDataID()
    {
        return GetComponent<DataDefinition>();
    }

    public void GetSaveDate(Data data)
    {
        data.SaveGameScene(currentLoadedScene);
    }

    public void LoadDate(Data data)
    {
        var playerID = playerTrans.GetComponent<DataDefinition>().ID;
        if (data.characterPosDict.ContainsKey(playerID))
        {
            positionToGo = data.characterPosDict[playerID];
            sceneToLoad = data.GetSavedScene();

            OnLoadRequestEvent(sceneToLoad, positionToGo, true);
        }
    }
}
