using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICoinCountText : MonoBehaviour
{
    private static Text coinCountText;

    private void Awake()
    {
        coinCountText = GetComponent<Text>();
    }

    public static void UpateText(int amount)
    {
        coinCountText.text = amount.ToString();
    }
}
