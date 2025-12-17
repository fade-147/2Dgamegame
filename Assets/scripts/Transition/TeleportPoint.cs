using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPoint : MonoBehaviour, IInteractable
{
    public SceneLoadEvent loadEventSO;
    public GameSceneSO sceneToGo;
    public Vector3 positionToGo;
    public void TriggerAction()
    {
        Debug.Log("´«ËÍ");
        loadEventSO.RaiseLoadRequestEvent(sceneToGo, positionToGo, true);
    }
}
