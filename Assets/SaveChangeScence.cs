using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveChangeScence : MonoBehaviour
{
    public VoidEventSO saveEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            saveEvent.RaiseEvent();
            Destroy (gameObject);
        }
    }
}
