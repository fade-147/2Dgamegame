using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{
    public GameObject newGameButton;

    private void OnEnable()
    {
        EventSystem .current .SetSelectedGameObject (newGameButton);  //让菜单的一开始就选择“开始游戏按钮”
    }

    public void ExitGame()
    {
        Debug.Log("Quit!");
        Application.Quit ();
    }
}
