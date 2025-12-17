using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro命名空间
using System.Collections;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI 引用")]
    public TMP_Text speakerNameText; 
    public TMP_Text contentText;
    public TMP_Text continueHintText; // 继续提示的Text
    public GameObject dialoguePanel;

    public Image speakerImage;       //说话人形象
    [Header("打字机设置")]
    public float typingSpeed = 0.05f; // 每个字的间隔时间

    private Dialogue currentDialogue;
    private int lineIndex;
    private bool isTyping = false; // 是否正在播放打字机效果
    private bool dialogueActive = false; // 对话是否正在进行


    private void Awake()
    {
        // 单例模式
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 场景切换时不销毁
        }
        else
        {
            Destroy(gameObject);
        }

        dialoguePanel.SetActive(false);
        continueHintText.gameObject.SetActive(false);
    }


    private void Update()
    {
        // 对话激活时，检测空格/鼠标点击继续
        if (dialogueActive && !isTyping)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame
            || Mouse.current.leftButton.wasPressedThisFrame)
            {
                DisplayNextSentence();
            }
        }
    }


    public void StartDialogue(Dialogue dialogue)
    {
        currentDialogue = dialogue;
        lineIndex = 0;
        dialogueActive = true;
        dialoguePanel.SetActive(true);

        // 显示第一行对话（包含形象+名字+文本）
        DisplayDialogueLine(currentDialogue.dialogueLines[lineIndex]);
        lineIndex++;
    }


    private void DisplayDialogueLine(Dialogue.DialogueLine line)
    {
        // 更新说话人名字
        speakerNameText.text = line.speakerName;

        // 更新说话人形象
        if (line.speakerSprite != null)
        {
            speakerImage.sprite = line.speakerSprite;
            speakerImage.gameObject.SetActive(true);
            //重置Image的大小
            speakerImage.preserveAspect = true;
        }
        else
        {
            speakerImage.gameObject.SetActive(false); // 无形象则隐藏
        }

        //播放打字机效果显示文本
        StartCoroutine(TypeSentence(line.sentenceContent));
    }

   
    public void DisplayNextSentence()
    {
        continueHintText.gameObject.SetActive(false);

        if (lineIndex < currentDialogue.dialogueLines.Length)
        {
            // 显示下一行对话（自动更新形象）
            DisplayDialogueLine(currentDialogue.dialogueLines[lineIndex]);
            lineIndex++;
        }
        else
        {
            EndDialogue();
        }
    }


    // 打字机效果的协程
    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        contentText.text = ""; // 清空之前的内容

        // 逐字添加
        foreach (char letter in sentence.ToCharArray())
        {
            contentText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        continueHintText.gameObject.SetActive(true); // 显示继续提示
    }


    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        continueHintText.gameObject.SetActive(false);
        speakerImage.gameObject.SetActive(false); // 隐藏形象
        dialogueActive = false;
        currentDialogue = null;
        lineIndex = 0;
    }
}
