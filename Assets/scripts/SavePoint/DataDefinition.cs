using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataDefinition : MonoBehaviour
{
    public PersistenType persistenType;
    public string ID;

    private void OnValidate()
    {
        if (persistenType == PersistenType.ReadWrite)
        {
            if (ID == string.Empty)
                ID = System.Guid.NewGuid().ToString();
        }
        else
        {
            ID=string .Empty;
        }
    }
}
