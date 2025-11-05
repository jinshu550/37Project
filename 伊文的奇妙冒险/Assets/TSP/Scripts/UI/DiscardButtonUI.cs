using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DiscardButtonUI : Singleton<DiscardButtonUI>
{
    [SerializeField] private Button confirmButton;
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform endPosition;
    [SerializeField] private float tweenDuration = 0.3f;
    public TextMeshProUGUI discardCountsText;
    private bool isInitialized = false;
    private Tween currentTween;
    private void OnEnable()
    {
        // 监听弃牌阶段的开始和结束
        ActionSystem.SubscribeReaction<DiscardGA>(OnDiscardPhaseStart, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<DiscardGA>(OnDiscardPhaseEnd, ReactionTiming.POST);
    }
    private void OnDisable()
    {
        ActionSystem.UnsubscribeReaction<DiscardGA>(OnDiscardPhaseStart, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<DiscardGA>(OnDiscardPhaseEnd, ReactionTiming.POST);
        CleanupTween();
    }
    private void Start()
    {
        InitializeReference();
    }
    private void InitializeReference()
    {
        if (isInitialized) return;
        if (confirmButton == null)
        {
            confirmButton = GetComponent<Button>();
        }
        if (confirmButton != null && startPosition != null)
        {
            confirmButton.transform.position = startPosition.position;
            confirmButton.onClick.AddListener(OnConfirmDiscard);
        }
        isInitialized = true;
    }
    private void Update()
    {
        if (!isInitialized)
            return;
        // 只有在弃牌阶段且选中了足够的卡牌时才启用按钮
        if (CardSystem.Instance.IsDiscarding)
        {
            confirmButton.interactable = CardSystem.Instance.selectedForDiscard.Count >= CardSystem.Instance.RequiredDiscardCount;
        }
    }

    private void OnConfirmDiscard()
    {
        // 通知CardSystem执行弃牌
        CardSystem.Instance.ExecuteDiscard();
        SoundSystem.Instance.PlaySound("SFX_CardShove");
    }

    private void OnDiscardPhaseStart(GameAction gameAction)
    {
        // 双重保险：检查引用是否有效且对象未被销毁
        if (this == null || !gameObject.activeInHierarchy) return;
        // 显示确认按钮
        AnimateButtonToPosition(endPosition.position);
    }

    private void OnDiscardPhaseEnd(GameAction gameAction)
    {
        // 双重保险：检查引用是否有效且对象未被销毁
        if (this == null || !gameObject.activeInHierarchy) return;
        // 隐藏确认按钮
        AnimateButtonToPosition(startPosition.position);
    }
    private void AnimateButtonToPosition(Vector3 destination)
    {
        // 确保之前的动画完成并清除
        CleanupTween();
        // 执行从起始位置到结束位置的平滑动画
        currentTween = confirmButton.transform.DOMove(destination, tweenDuration)
            .SetEase(Ease.OutQuad)
            .OnKill(() => currentTween = null); // 设置动画曲线，可根据需要调整
    }
    private void CleanupTween()
    {
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
            currentTween = null;
        }
    }
    public void HintDiscount(int selectedCard, int requiredDiscardCount)
    {
        discardCountsText.text = $"{selectedCard}/{requiredDiscardCount}";
    }
}
