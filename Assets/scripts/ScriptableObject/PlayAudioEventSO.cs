using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

[CreateAssetMenu(menuName ="Event/playAudioEventSO")]
public class PlayAudioEventSO : ScriptableObject
{
    public UnityAction<AudioClip> OnEventRaised;
    public void RaiseEvent(AudioClip audioClip)
    {
        OnEventRaised?.Invoke (audioClip);
    }
}
