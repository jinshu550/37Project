using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : ExploreSingleton<InventoryManager>
{
    [Header("物品数据库")]
    [Tooltip("背包卡牌数据库")]
    public CardBasicDatabase bagCarDB;

    [Tooltip("所有功能卡牌数据库")]
    public CardBasicDatabase allFunctionCardDB;

    [Header("所有身材卡牌数据库")]
    [SerializeField] private BodyType_SO body1CardDB; // 拖入基础身材数据库实例
    [SerializeField] private BodyType_SO body2CardDB; // 拖入身材2数据库实例
    [SerializeField] private BodyType_SO body3CardDB; // 拖入身材3数据库实例


    protected override void Awake()
    {
        base.Awake(); // 调用基类的单例初始化
    }

    void OnEnable()
    {

        EventHandler.OnAllDataReset += OnDataReset; // 监听数据重置
    }

    // 添加缺失的 OnDataReset 方法


    void OnDisable()
    {
        EventHandler.OnAllDataReset -= OnDataReset;
    }

    #region  对数据库的处理
    /// <summary>添加物品到背包
    /// </summary>
    /// <param name="Card"></param>
    /// <param name="toDestory"></param>
    public bool AddCardToBack(int cardId)
    {
        //查找所有功能卡牌找到对应id的牌
        CardBasicInformation targetCard = GetFunctionCard(cardId);
        // 检查物品是否已在背包数据库中
        bool exists = bagCarDB.allExtendedCards.Exists(item => item.cardId == cardId);
        if (!exists)
        {
            //克隆新卡牌
            CardBasicInformation newItem = CloneCard(targetCard);
            newItem.card = targetCard.card;
            bagCarDB.allExtendedCards.Add(newItem);

            //在添加后进行更新缓存字典
            if (bagCarDB != null)
            {
                bagCarDB.InitializeExtendedCardDictionary();

            }

            return true;
        }

        return false;
    }

    /// <summary>克隆卡牌
    /// </summary>
    /// <param name="card">需要克隆的卡牌数据</param>
    /// <returns></returns>
    private CardBasicInformation CloneCard(CardBasicInformation card)
    {
        return new CardBasicInformation
        {
            cardId = card.cardId,
            cardName = card.cardName,
            description = card.description,
            battleSprite = card.battleSprite,
            mapSprite = card.mapSprite,
            requiredCardIds = new List<int>(card.requiredCardIds), // 深拷贝列表
        };
    }

    private void OnDataReset()
    {
        // 根据需要重置背包或其他数据
        if (bagCarDB != null)
        {
            bagCarDB.allExtendedCards.Clear();
            bagCarDB.InitializeExtendedCardDictionary();
        }
        EventHandler.CallUpdateInventoryUI(GetBackAllFunctionCards());
    }

    #endregion

    #region 外部获取数据库信息
    /// <summary>通过ID 从功能卡牌数据库 查找卡牌
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public CardBasicInformation GetFunctionCard(int cardId)
    {
        return allFunctionCardDB.GetExtendedCardById(cardId);
    }

    /// <summary>通过ID 从背包卡牌数据库 查找卡牌
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public CardBasicInformation GetBackFunctionCard(int cardId)
    {
        if (bagCarDB == null)
        {
            return null;
        }
        return bagCarDB.GetExtendedCardById(cardId);
    }

    /// <summary>获取背包所有功能卡牌
    /// </summary>
    /// <returns></returns>
    public List<CardBasicInformation> GetBackAllFunctionCards()
    {
        if (bagCarDB == null)
            return new List<CardBasicInformation>();
        return new List<CardBasicInformation>(bagCarDB.allExtendedCards);
    }

    /// <summary>获取所有身材1卡牌（转换为CardBasicInformation类型）
    /// </summary>
    /// <returns></returns>
    public List<CardBasicInformation> GetBodyType1Cards()
    {
        return ConvertBodyListToCardList(body1CardDB.allBodyCards);
    }

    /// <summary>获取所有身材2卡牌
    /// </summary>
    /// <returns></returns>
    public List<CardBasicInformation> GetBodyType2Cards()
    {
        return ConvertBodyListToCardList(body2CardDB.allBodyCards);
    }

    /// <summary>获取所有身材3卡牌
    /// </summary>
    /// <returns></returns>
    public List<CardBasicInformation> GetBodyType3Cards()
    {
        return ConvertBodyListToCardList(body3CardDB.allBodyCards);
    }

    #region 将BodyType 列表转换为 CardBasicInformation 列表
    /// <summary>将BodyType列表转换为 CardBasicInformation列表
    /// </summary>
    /// <param name="bodyList">需要转换的身材列表</param>
    /// <returns></returns>
    private List<CardBasicInformation> ConvertBodyListToCardList(List<BodyType> bodyList)
    {
        List<CardBasicInformation> result = new List<CardBasicInformation>();
        foreach (var body in bodyList)
        {
            result.Add(ConvertBodyToCardBasic(body));
        }
        return result;
    }

    /// <summary>将 BodyType列表里的每个数据 转换为 CardBasicInformation（适配背包数据结构）
    /// </summary>
    /// <param name="bodyCard">身材卡基础数据</param>
    /// <returns>转换后的扩展属性卡牌数据</returns>
    private CardBasicInformation ConvertBodyToCardBasic(BodyType bodyCard)
    {
        // 新建 CardBasicInformation 实例
        CardBasicInformation card = new CardBasicInformation();

        // 复制父类（BodyType）的基础属性
        card.cardId = bodyCard.cardId;
        card.cardName = bodyCard.cardName;
        card.description = bodyCard.description;
        card.battleSprite = bodyCard.battleSprite;

        // 初始化子类特有有的扩展属性（身材卡可能没有这些属性，设为默认值）
        card.mapSprite = null; // 身材卡没有地图精灵
        card.requiredCardIds = new List<int>(); // 身材卡没有合成配方，设为空列表

        return card;
    }
    #endregion
    #endregion


}
