// CanvasHelper.cs
using UnityEngine;

public class CanvasHelper : MonoBehaviour
{
    public static CanvasHelper Instance;

    public RectTransform CanvasRect;
    public Camera Camera;
    public Transform CanvasTransform;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // 自动获取Canvas组件
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            CanvasRect = canvas.GetComponent<RectTransform>();
            Camera = canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null;
            CanvasTransform = canvas.transform;
        }
    }
}