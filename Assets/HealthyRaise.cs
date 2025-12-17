using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthyRaise : MonoBehaviour
{
    public CharacterEvent CharacterEventSO;

    private void OnTriggerStay2D(Collider2D other)
    {
        Character Player=other.GetComponent<Character>();
        if (other.GetComponent<Character>() != null)
        {
            if (Player.currentHealth < Player.maxHealth)
            {
                Player.currentHealth += Time.deltaTime*3;
                CharacterEventSO.RaisedEvent(Player);
            }
        }
    }
}
