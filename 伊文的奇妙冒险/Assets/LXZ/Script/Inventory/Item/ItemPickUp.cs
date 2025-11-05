using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class ItemPickUp : MonoBehaviour
{
    public string targetTag = "Card";//检测标签避免点击其他地方
    public string synthesizedCardName = "展示合成卡牌"; // 合成预设体的固定名字
    public LayerMask targetLayer;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
        //如果未赋值相机，自动获取主相机
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        // 非探索场景不响应拾取
        if (!SceneManager.GetActiveScene().name.StartsWith(GameManager.Instance.exploreScenePrefix))
            return;

        // 检测鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            // 忽略UI点击（如果点击了按钮等UI元素，不执行销毁逻辑）
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            // 从相机发射射线到鼠标位置
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            // 转换为2D射线检测（检测2D碰撞体）
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, targetLayer);
            // 如果射线命中了物体
            if (hit)
            {
                // 检查是否是Card标签的物品 ，并和精灵亡者第一次对话结束
                if (hit.collider.CompareTag(targetTag) && !GameDataManager.Instance.IsElvenUndeadFirstMeeting())
                {

                    Item item = hit.collider.GetComponent<Item>();
                    if (item != null)
                    {

                        // 添加到背包  返回是否添加成功并销毁物体
                        bool toDestory = InventoryManager.Instance.AddCardToBack(item.cardID);

                        if (toDestory)
                        {
                            item.itemSprite.SetActive(false); // 先隐藏，避免重复点击
                            var currentBackpackData = InventoryManager.Instance.GetBackAllFunctionCards();
                            EventHandler.CallUpdateInventoryUI(currentBackpackData);
                            EventHandler.CallOnItemCollected(item.cardName);
                        }
                    }
                }

            }
        }
    }

    /// <summary>
    /// 处理UI合成卡牌（Image）的收集（由UI点击事件触发）
    /// </summary>
    public void CollectUISynthesizedCard(SynthesizedCard synthesizedCard)
    {
        if (synthesizedCard == null)
        {
            return;
        }

        // 验证UI卡牌名字（确保是目标合成卡牌）
        GameObject uiCardObj = synthesizedCard.gameObject;
        if (!uiCardObj.name.Equals(synthesizedCardName))
        {
            return;
        }

        // 执行收集逻辑
        CollectCard(synthesizedCard.cardID, uiCardObj);
    }

    /// <summary>
    /// 统一的卡牌收集逻辑（兼容普通卡牌和UI合成卡牌）
    /// </summary>
    /// <param name="cardID">卡牌ID</param>
    /// <param name="cardObj">要销毁的卡牌对象（物理卡牌/UI卡牌）</param>
    private void CollectCard(int cardID, GameObject cardObj)
    {
        if (cardID <= 0 || cardObj == null || InventoryManager.Instance == null)
        {
            return;
        }

        // 添加到背包
        bool isAdded = InventoryManager.Instance.AddCardToBack(cardID);
        if (isAdded)
        {
            Destroy(cardObj); // 收集成功，销毁卡牌对象（物理/

            // 刷新背包UI
            var currentBackpack = InventoryManager.Instance.GetBackAllFunctionCards();
            EventHandler.CallUpdateInventoryUI(currentBackpack);
        }
        else
        {
        }
    }
}


