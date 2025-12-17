using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveable            //½Ó¿Ú
{
    DataDefinition GetDataID();
    void RegisterSaveDate()=>DateManager.instance.RegisterSaveData(this);
    
    void UnregisterSaveDate()=>DateManager.instance.unRegisterSaveData(this);
    void GetSaveDate(Data data);
    void LoadDate(Data data);
}
