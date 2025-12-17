using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder (order:-100)]
public class DateManager : MonoBehaviour
{
    public static DateManager instance;
    [Header("事件监听")]
    public VoidEventSO saveDataEvent;
    public VoidEventSO loadDataEvent;

    private List<ISaveable > saveableList = new List<ISaveable> ();     //创建列表
    private Data saveData;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        saveData =new Data();
    }

    public void OnEnable()
    {
        saveDataEvent.OnEventRaised += Save;
        loadDataEvent.OnEventRaised += Load;
    }

    public void OnDisable()
    {
        saveDataEvent.OnEventRaised -= Save;
        loadDataEvent.OnEventRaised -= Load;
    }

    private void Update()
    {
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            Load();
        }
    }
    public void RegisterSaveData(ISaveable savedata)
    {
        if(!saveableList .Contains(savedata))
        {
            saveableList.Add(savedata);
        }
    }

    public void unRegisterSaveData(ISaveable saveable)
    {
        saveableList.Remove (saveable);
    }

    public void Save()
    {
        foreach(var saveable in saveableList )
        {
            saveable.GetSaveDate(saveData);
        }

        foreach(var item in saveData.characterPosDict)
        {
            Debug.Log(item.Key + "   " + item.Value);
        }
    }

    public void Load()
    {
        foreach (var saveable in saveableList)
        {
            saveable.LoadDate(saveData);
        }
    }
        
}
