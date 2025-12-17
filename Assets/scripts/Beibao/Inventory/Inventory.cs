using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/New Inventory")]
public class Inventory : ScriptableObject 
{  //ÁÐ±í£¨±³°ü£©
    public List<Item> itemList = new List<Item>();
}
