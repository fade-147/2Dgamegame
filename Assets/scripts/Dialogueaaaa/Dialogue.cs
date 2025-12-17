using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "New Dialogue",
    menuName = "Dialogue System/Dialogue", // 对应Project窗口的创建路径
    order = 0
)]
public class Dialogue : ScriptableObject
{
    [Tooltip("每一行对话的完整信息（名字+形象+文本）")]
    public DialogueLine[] dialogueLines;

    // 可序列化的单句对话类（会显示在Inspector面板）
    [System.Serializable]
    public class DialogueLine
    {
        public string speakerName; // 说话人名字
        public Sprite speakerSprite; // 说话人形象（Sprite）
        [TextArea(3, 10)]
        public string sentenceContent; // 对话文本
    }
}
