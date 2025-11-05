using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CardView : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    //[SerializeField] private TMP_Text mana;
    [SerializeField] private SpriteRenderer imageSR;
    [SerializeField] private GameObject wrapper;
    [SerializeField] private GameObject discardHighlight; // 弃牌高亮效果
    [SerializeField] private LayerMask DropLayerMask;
    [SerializeField] private Material changedMaterial;
    private Material initMaterial;
    SpriteRenderer SR;
    public Vector3 dragStartPosition;
    public Quaternion dragStartRotation;
    public Card Card { get; private set; }
    void Start()
    {
        SR = GetComponentInChildren<SpriteRenderer>();
        initMaterial = SR.material;
    }
    public void SetUp(Card card)
    {
        Card = card;
        title.text = card.Title;
        description.text = card.Description;
        //mana.text = card.Mana.ToString();
        imageSR.sprite = card.Image;
    }
    private void OnMouseEnter()
    {
        if (!Interaction.Instance.PlayerCanHover()) return;
        wrapper.SetActive(false);
        Vector3 pos = new Vector3(transform.position.x, -2, 0);
        CardViewHoverSystem.Instance.Show(Card, pos);
    }
    private void OnMouseExit()
    {
        if (!Interaction.Instance.PlayerCanHover()) return;
        CardViewHoverSystem.Instance.Hide();
        wrapper.SetActive(true);
    }
    private void OnMouseDown()
    {
        if (!Interaction.Instance.PlayerCanInteract()) return;
        //如果是弃牌阶段，点击卡牌用于选择/取消选择卡牌
        if (CardSystem.Instance.IsDiscarding)
        {
            bool isSelected = CardSystem.Instance.ToggleCardForDiscard(Card);
            if (isSelected)
            {
                //高亮效果
                SR.material = changedMaterial;
            }
            else
            {
                //取消高亮
                SR.material = initMaterial;
            }
            return;
        }
        if (Card.ManualTargetEffect != null)
        {
            ManualTargetingSystem.Instance.StartTargeting(transform.position);
        }
        else
        {
            Interaction.Instance.PlayerIsDragging = true;
            wrapper.SetActive(true);
            CardViewHoverSystem.Instance.Hide();
            dragStartPosition = transform.position;
            dragStartRotation = transform.rotation;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.position = MouseUtil.GetMousePositionInWorldSpace(-1);
        }
    }
    private void OnMouseDrag()
    {
        if (!Interaction.Instance.PlayerCanInteract()) return;
        if (Card.ManualTargetEffect != null) return;
        transform.position = MouseUtil.GetMousePositionInWorldSpace(-1);
    }
    private void OnMouseUp()
    {
        if (!Interaction.Instance.PlayerCanInteract()) return;
        if (Card.ManualTargetEffect != null)
        {
            EnemyView target = ManualTargetingSystem.Instance.EndTargeting(MouseUtil.GetMousePositionInWorldSpace(-1));
            if (target != null && ManaSystem.Instance.HasEnoughMana(Card.Mana))
            {
                PlayCardGA playCardGA = new(Card, target);
                ActionSystem.Instance.Perform(playCardGA);
            }
        }
        else
        {
            if (Physics.Raycast(transform.position, Vector3.forward, out RaycastHit hit, 10f, DropLayerMask) && ManaSystem.Instance.HasEnoughMana(Card.Mana))
            {
                //Play Card
                PlayCardGA playCardGA = new(Card);
                ActionSystem.Instance.Perform(playCardGA);
            }
            else
            {
                transform.position = dragStartPosition;
                transform.rotation = dragStartRotation;
            }
            Interaction.Instance.PlayerIsDragging = false;

        }

    }
}
