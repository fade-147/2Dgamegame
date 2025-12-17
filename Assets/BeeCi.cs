using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeCi : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy"))
        {
            Destroy(this.gameObject);
        }
            
        
    }
   
}
