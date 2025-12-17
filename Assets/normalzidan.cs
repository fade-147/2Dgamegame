using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class normalzidan : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if ( !other.CompareTag("Player"))
        {
            Debug.Log(other.gameObject);
            StartCoroutine(Wait());
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.05f);
        Destroy (this.gameObject);
    }
}
