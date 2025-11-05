using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NPCDialogueSystem : MonoBehaviour
{
    #region 参数设置
    [Header("对话配置")]
    [SerializeField] public string npcName; //NPC名称
    [SerializeField] private int GetCardID; //NPC赠送物品ID
    private CardBasicInformation CardDetails;

    [Header("UI组件")]
    [SerializeField] private GameObject DialoguePanel;   //对话面板
    [SerializeField] private GameObject playerDialogBox;   //玩家对话框UI
    [SerializeField] private GameObject npcDialogBox;  //NPC对话框UI
    [SerializeField] private GameObject narratorDialogBox; //旁白对话框UI
    [SerializeField] private TextMeshProUGUI playerText;   //玩家文本显示组件
    [SerializeField] private TextMeshProUGUI npcText;  //NPC文本显示组件
    [SerializeField] private TextMeshProUGUI narratorText; //旁白文本显示组件
    [Header("有选项隐藏图标")]
    [SerializeField] private GameObject dialogueico;

    [Header("对话数据")]
    public DialogueLine[] firstMeetingDialogue; // 首次对话内容（只触发一次）
    public DialogueLine[] repeatDialogue;      // 重复对话内容（首次对话后触发）

    [Header("对话框显示与否的控制")]
    [SerializeField] private CanvasGroup playerCanvasGroup; // 玩家对话框的CanvasGroup
    [SerializeField] private CanvasGroup npcCanvasGroup;    // NPC对话框的CanvasGroup
    [SerializeField] private CanvasGroup narratorCanvasGroup;    // 旁白对话框的CanvasGroup
    [SerializeField] private float fadeDuration = 0.3f; // 淡入淡出持续时间

    [Header("选项UI")]
    [SerializeField] private GameObject optionsPanel; // 选项面板
    [SerializeField] private GameObject optionButtonPrefab; // 选项按钮预制体

    [Header("根据选项消耗敌人卡牌")]
    [SerializeField] public CardData enemyBuff1; //怪物功能牌
    [SerializeField] public CardData enemyBuff2; //怪物功能牌

    private bool isFirstMeeting = true; // 是否为首次对话
    public bool isDialogueActive = false; // 对话是否正在进行
    private int currentLineIndex = 0; // 当前对话行索引
    private DialogueLine[] currentDialogue; // 当前对话数据

    #endregion

    #region 开始对话/显示对话
    /// <summary>
    /// 开始对话
    /// </summary>
    public void StartDialogueByEvent()
    {
        if (isDialogueActive) return;

        isDialogueActive = true;
        // 触发「对话开始事件」→ 锁定输入
        EventHandler.CallOnDialogueStateChanged(true);
        DialoguePanel.SetActive(true);
        StartCoroutine(ProcessDialogue());
    }

    /// <summary> 显示对话 </summary>
    private IEnumerator ProcessDialogue()
    {
        if (npcName == "精灵亡者" && !GameDataManager.Instance.IsElvenUndeadFirstMeeting())
        {
            isFirstMeeting = false;
        }
        currentDialogue = isFirstMeeting ? firstMeetingDialogue : repeatDialogue;
        currentLineIndex = 0;

        // 安全检查：对话数据为空时直接结束
        if (currentDialogue == null || currentDialogue.Length == 0)
        {
            playerDialogBox.SetActive(false);
            npcDialogBox.SetActive(false);
            yield break;
        }

        while (currentLineIndex < currentDialogue.Length)
        {
            dialogueico.SetActive(true);
            DialogueLine line = currentDialogue[currentLineIndex];
            // 显示对话
            if (line.speaker == "伊文")
                ShowPlayerDialog(line.text);
            else if (line.speaker == "旁白")
            {
                ShowNarratorDialog(line.text);
            }
            else
                ShowNpcDialog(line.text);

            yield return new WaitForSeconds(0.3f);

            // 如果有选项，显示选项UI，等待玩家选择
            if (line.options != null && line.options.Count > 0)
            {
                dialogueico.SetActive(false);
                yield return ShowOptionsAndWait(line.options);
                continue;
            }
            else
            {
                yield return new WaitUntil(() => Input.GetMouseButton(0) || !isDialogueActive);
            }

            // 隐藏对话
            if (line.speaker == "伊文")
                HidePlayerDialog();
            else if (line.speaker == "旁白")
                HideNarratorDialog();
            else
                HideNpcDialog();

            yield return new WaitForSeconds(0.3f);
            currentLineIndex++;
        }
        //对话结束-->解锁输入
        EventHandler.CallOnDialogueStateChanged(false);
        DialoguePanel.SetActive(false);
        isDialogueActive = false;
        isFirstMeeting = false;
        GetIsDialogueActive(); //对话结束后调用判斷npc是否有獲得物品
    }

    #endregion

    #region  对话选项处理
    /// <summary> 显示选项并等待玩家选择，处理道具消耗与分支跳转 </summary>
    private IEnumerator ShowOptionsAndWait(List<DialogueOption> options)
    {
        // 清空旧选项
        foreach (Transform child in optionsPanel.transform)
            Destroy(child.gameObject);
        optionsPanel.SetActive(true);

        bool optionSelected = false;
        int selectedIndex = -1;

        // 动态生成按钮
        for (int i = 0; i < options.Count; i++)
        {
            int idx = i;
            GameObject btnObj = Instantiate(optionButtonPrefab, optionsPanel.transform);
            TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null) btnText.text = options[i].optionText;
            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                // 检查道具
                if (options[idx].requiredItemId != 0)
                {
                    if (PlayerHasItem(options[idx].requiredItemId))
                    {
                        // 可弹窗提示“缺少道具”
                        EventHandler.CallOnLackItem(options[idx].requiredItemId);
                        StaticUtility.CurrentEnemyBuff = null;
                        isDialogueActive = false;

                        //隐藏对话框
                        playerDialogBox.SetActive(false);
                        npcDialogBox.SetActive(false);
                        narratorDialogBox.SetActive(false);
                        DialoguePanel.SetActive(false);

                        return;
                    }
                    if (enemyBuff1 != null)
                        StaticUtility.CurrentEnemyBuff = enemyBuff1;//消耗道具触发怪物功能牌1

                }
                else
                {
                    if (enemyBuff2 != null)
                    {
                        StaticUtility.CurrentEnemyBuff = enemyBuff2;//不消耗道具触发怪物功能牌2
                    }

                }
                selectedIndex = idx;
                optionSelected = true;
            });
        }

        // 等待玩家选择
        yield return new WaitUntil(() => optionSelected || !isDialogueActive);

        optionsPanel.SetActive(false);


        if (selectedIndex >= 0)
        {

            // 切换到新对话流
            currentDialogue = options[selectedIndex].nextDialogue;
            currentLineIndex = 0;
        }
        else
        {
            // 未选择，默认进入下一句
            currentLineIndex++;
        }
    }

    // 检查玩家是否有指定道具
    private bool PlayerHasItem(int itemid)
    {
        //查找卡牌是否为空 ，为空就返回true
        return InventoryManager.Instance.GetBackFunctionCard(itemid) == null;
    }

    #endregion

    #region 对话显示与隐藏
    /// <summary> 播放玩家对话
    /// </summary>
    /// <param name="text"></param>
    private void ShowPlayerDialog(string text)
    {
        npcDialogBox.SetActive(false);
        narratorDialogBox.SetActive(false);
        playerDialogBox.SetActive(true);
        // 设置玩家对话文本
        playerText.text = text;
        // 激活玩家对话框并触发显示动画
        playerDialogBox.SetActive(true);
        StartCoroutine(FadeCanvasGroup(playerCanvasGroup, 1f));
    }

    /// <summary> 显示NPC对话框
    /// </summary>
    /// <param name="text"></param>
    private void ShowNpcDialog(string text)
    {
        playerDialogBox.SetActive(false);
        narratorDialogBox.SetActive(false);
        // 设置NPC对话文本
        npcText.text = text;
        // 激活NPC对话框并触发显示动画
        npcDialogBox.SetActive(true);
        StartCoroutine(FadeCanvasGroup(npcCanvasGroup, 1f));
    }

    /// <summary> 显示旁白对话框
    /// </summary>
    /// <param name="text"></param>
    private void ShowNarratorDialog(string text)
    {
        playerDialogBox.SetActive(false);
        npcDialogBox.SetActive(false);
        narratorDialogBox.SetActive(true);
        // 设置NPC对话文本
        narratorText.text = text;
        // 激活NPC对话框并触发显示动画
        narratorDialogBox.SetActive(true);
        StartCoroutine(FadeCanvasGroup(narratorCanvasGroup, 1f));
    }

    /// <summary> 隐藏玩家对话框 </summary>
    private void HidePlayerDialog()
    {
        StartCoroutine(FadeCanvasGroup(playerCanvasGroup, 0f));
    }

    /// <summary>  隐藏NPC对话框 </summary>
    private void HideNpcDialog()
    {
        StartCoroutine(FadeCanvasGroup(npcCanvasGroup, 0f));
    }

    /// <summary> 隐藏旁白对话框 </summary>
    private void HideNarratorDialog()
    {
        StartCoroutine(FadeCanvasGroup(narratorCanvasGroup, 0f));
    }

    /// <summary> 淡入淡出CanvasGroup
    /// </summary>
    /// <param name="cg"></param>
    /// <param name="targetAlpha"></param>
    /// <returns></returns>
    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float targetAlpha)
    {
        float startAlpha = cg.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }

        cg.alpha = targetAlpha;

        switch (cg)
        {
            case var _ when cg == playerCanvasGroup:
                if (targetAlpha == 0f)
                    playerDialogBox.SetActive(false);
                break;
            case var _ when cg == npcCanvasGroup:
                if (targetAlpha == 0f)
                    npcDialogBox.SetActive(false);
                break;
            case var _ when cg == narratorCanvasGroup:
                if (targetAlpha == 0f)
                    narratorDialogBox.SetActive(false);
                break;
        }

    }

    #endregion
    public bool GetIsFirstMeeting()
    {
        return isFirstMeeting;
    }

    //對話結束后調用判斷npc是否有獲得物品
    public void GetIsDialogueActive()
    {
        switch (npcName)
        {
            case "宴会精灵":

                CardDetails = InventoryManager.Instance.GetFunctionCard(GetCardID);
                // 添加到背包  返回是否添加成功并销毁物体
                bool toDestory = InventoryManager.Instance.AddCardToBack(GetCardID);

                if (toDestory)
                {
                    var currentBackpackData = InventoryManager.Instance.GetBackAllFunctionCards();
                    EventHandler.CallUpdateInventoryUI(currentBackpackData);
                    EventHandler.CallOnItemCollected(CardDetails != null ? CardDetails.cardName : "[未知物品]");
                }
                break;

            default:
                break;
        }
    }
}

[Serializable]
public class DialogueOption
{
    public string optionText; // 选项文本
    [SerializeReference]
    public DialogueLine[] nextDialogue; // 选中后进入的新对话流
    public int requiredItemId; // 需要的道具id
}

[Serializable]
public class DialogueLine
{
    public string speaker; // 说话人的名字
    [TextArea(3, 5)] public string text; // 对话文本
    public List<DialogueOption> options; // 分支选项（可为空）

    public DialogueLine() { }
}
