using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeManager : MonoBehaviour
{
    public Button LeftButton;
    public Button RightButton;
    public GameObject LeftPanel;
    public GameObject RightPanel;
    private void Awake()
    {
        LeftButton.onClick.AddListener(ToggleLeftButton);
        RightButton.onClick.AddListener(ToggleRightButton);
    }
    private void ToggleLeftButton()
    {
        LeftPanel .SetActive(true);
        RightPanel .SetActive(false);
    }
    private void ToggleRightButton()
    {
        LeftPanel .SetActive(false);
        RightPanel .SetActive(true);
    }
}
