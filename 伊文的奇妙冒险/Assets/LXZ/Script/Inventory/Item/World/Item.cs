using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Item : MonoBehaviour
{
    [Header("每个物品的配置")]
    public int cardID; //物品id
    public string cardName; //物品name
    public GameObject itemSprite; // item物品
    public GameObject itemLightPar; // item物品

    [SerializeField] private SpriteRenderer spriteRenderer;  //找到对应的精灵图
    [SerializeField] private BoxCollider2D coll;  //对应碰撞体大小
    [SerializeField] private Light2D light2D;     // 子物体灯光组件
    [SerializeField] private ParticleSystem particle;  // 子物体粒子系统

    [Header("数据类引用")]
    public CardBasicInformation CardDetails;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (coll == null)
            coll = GetComponent<BoxCollider2D>();
        if (light2D == null)
            light2D = transform.Find("Light 2D").GetComponentInChildren<Light2D>();
        if (particle == null)
        {
            particle = transform.Find("Particle System").GetComponentInChildren<ParticleSystem>();
        }

        if (InventoryManager.Instance != null && cardID != 0)
        {

            //如果背包中有物品就隐藏
            if (InventoryManager.Instance.GetBackFunctionCard(cardID) != null)
            {
                itemSprite.SetActive(false);
            }
            else if (InventoryManager.Instance.GetBackFunctionCard(cardID) == null)
            {
                itemSprite.SetActive(true);
            }

            if (GameDataManager.Instance.IsElvenUndeadFirstMeeting())
            {
                itemLightPar.SetActive(false);
            }
            else
            {
                itemLightPar.SetActive(true);
            }
        }
    }

    private void Start()
    {
        if (cardID != 0)
        {
            Init(cardID);
        }
    }


    /// <summary> 初始化物品信息
    /// </summary>
    /// <param name="ID"></param>
    public void Init(int ID)
    {
        gameObject.name = "Item_" + ID; //重命名物体，方便识别
        gameObject.tag = "Card"; //设置标签为card，方便射线检测
        gameObject.layer = LayerMask.NameToLayer("Collect"); //设置图层为 Collect ，方便射线检测

        cardID = ID;

        //通过Inventory获得当前信息
        CardDetails = InventoryManager.Instance.GetFunctionCard(cardID);

        cardName = CardDetails != null ? CardDetails.cardName : "[未知物品]";

        //如果不为空，则显示图片
        if (CardDetails != null)
        {
            spriteRenderer.sprite = CardDetails.mapSprite != null ? CardDetails.mapSprite : CardDetails.battleSprite;
            //修改碰撞体尺寸
            Vector2 newSize = new Vector2(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y);//图片的尺寸
            coll.size = newSize;
            coll.offset = new Vector2(0, spriteRenderer.sprite.bounds.center.y);

            // 调整灯光和粒子系统
            AdjustLightAndParticle(newSize);
        }

    }

    /// <summary> 根据精灵图尺寸调整灯光和粒子系统
    /// </summary>
    /// <param name="spriteSize">精灵图的大小</param>
    private void AdjustLightAndParticle(Vector2 spriteSize)
    {
        // 调整Light2D（点光源模式）
        if (light2D != null)
        {
            // 确保灯光类型是点光源（如果需要其他类型可调整）
            if (light2D.lightType != Light2D.LightType.Point)
            {
                light2D.lightType = Light2D.LightType.Point;
            }
            //光源发光直径是图片的x
            light2D.pointLightInnerRadius = spriteSize.x / 2f;
            light2D.pointLightOuterRadius = spriteSize.x;

        }
        else
        {
            Debug.LogWarning($"Light_Effect下未找到Light2D组件", this);
        }

        // 调整ParticleSystem
        if (particle != null)
        {
            // 获取粒子系统的形状模块
            var shape = particle.shape;

            shape.radius = spriteSize.x / 2f;

        }
        else
        {
            Debug.LogWarning($"Light_Effect下未找到ParticleSystem组件", this);
        }
    }

    //如果背包中有物品就隐藏
    void Update()
    {
        if (GameDataManager.Instance.IsElvenUndeadFirstMeeting() && itemLightPar.activeSelf)
        {
            itemLightPar.SetActive(false);
        }
        else if (!GameDataManager.Instance.IsElvenUndeadFirstMeeting())
        {
            itemLightPar.SetActive(true);
        }
    }
}
