using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flyeyejiaocheng : MonoBehaviour
{
  private Character character;
    public GameObject Bound;
    private void Awake()
    {
        character = GetComponent<Character>();
    }
    private void Update()
    {
        if(character.currentHealth < 10)
        {
            Bound.SetActive (false);
        }
    }
}
