using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class SettlementUI : MonoBehaviour
{
    [SerializeField] private RectTransform targetUI;
    [SerializeField] private Button winButton;
    [SerializeField] private Button againButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private Text winText;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Text againText;
    [SerializeField] private Text returnText;
    [SerializeField] private Vector2 initialPosition;
    [SerializeField] private Vector2 targetPosition;
    [SerializeField] private Image backgroundMask;
    private CanvasGroup canvasGroup;
    private float animationDuration = 0.5f;
    void Start()
    {

        transform.gameObject.SetActive(false);

        winText.text = "确认";
        againText.text = "再来一次";
        returnText.text = "回到世界";
        targetUI.position = initialPosition;

        //隐藏按钮
        winButton.gameObject.SetActive(false);
        againButton.gameObject.SetActive(false);
        returnButton.gameObject.SetActive(false);


        //绑定按钮事件
        winButton.onClick.AddListener(OnWinbuttonClick);
        againButton.onClick.AddListener(OnReturnbuttonClick);
        returnButton.onClick.AddListener(OnAgainbuttonClick);

        canvasGroup = backgroundMask.GetComponent<CanvasGroup>();
    }
    //下拉动画
    public void MoveSettlementUIDown(bool win)
    {
        AudioManager.Instance.ToggleBGMPause();
        transform.gameObject.SetActive(true);
        // canvasGroup.alpha = 1;
        // canvasGroup.blocksRaycasts = true;
        targetUI.DOKill();
        targetUI.DOAnchorPos(targetPosition, animationDuration).SetEase(Ease.OutQuad);
        if (win && !againButton.gameObject.activeSelf && !returnButton.gameObject.activeSelf)
        {
            resultText.text = "游戏胜利";
            winButton.gameObject.SetActive(true);
        }
        else
        {
            resultText.text = "游戏失败";
            againButton.gameObject.SetActive(true);
            returnButton.gameObject.SetActive(true);
        }
    }
    private IEnumerator MoveSettlementUIUp()
    {
        targetUI.DOKill();
        Tween tween = targetUI.DOAnchorPos(initialPosition, animationDuration).SetEase(Ease.InQuad);
        yield return tween.WaitForCompletion();
        // canvasGroup.alpha = 0;
        // canvasGroup.blocksRaycasts = false;
        winButton.gameObject.SetActive(false);
        againButton.gameObject.SetActive(false);
        returnButton.gameObject.SetActive(false);
    }
    private void OnWinbuttonClick()
    {
        MoveSettlementUIUp();
        BattleSceneSwitcher.ReturnToPreviousScene();
    }
    private void OnReturnbuttonClick()
    {
        MoveSettlementUIUp();
        BattleSceneSwitcher.ReturnToPreviousScene();
    }
    private void OnAgainbuttonClick()
    {
        MoveSettlementUIUp();
        BattleSceneSwitcher.ReturnToPreviousScene();
    }
}
