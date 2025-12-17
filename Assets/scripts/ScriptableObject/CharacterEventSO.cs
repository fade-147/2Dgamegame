using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/CharacterEventSO")]
public class CharacterEvent : ScriptableObject
{
    public UnityAction<Character> OnEventRaised;

    public void RaisedEvent(Character character)
    {
        OnEventRaised?.Invoke(character);
    }

}
