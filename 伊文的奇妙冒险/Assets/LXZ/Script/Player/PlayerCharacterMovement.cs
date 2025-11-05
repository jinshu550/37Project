using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;          // 移动速度

    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Animator _anim;
    [SerializeField] private SpriteRenderer spriteRenderer; //翻转的图片
    private float horizontalInput;  //输入

    private bool isMovementEnabled = true;// 移动是否启用
    private bool isMoving = false;  // 是否正在移动
    private void Awake()
    {
        if (_rb == null)
            _rb = GetComponent<Rigidbody2D>();
        if (_anim == null)
            _anim = GetComponent<Animator>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        // 监听对话事件：控制移动开关
        EventHandler.OnDialogueStateChanged += OnDialogueStateChanged;
    }

    private void OnDisable()
    {
        // 移除监听
        EventHandler.OnDialogueStateChanged -= OnDialogueStateChanged;
    }

    void Update()
    {
        if (isMovementEnabled)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
        }
        else
        {
            horizontalInput = 0;
        }
    }

    private void FixedUpdate()
    {
        if (!isMovementEnabled)
        {
            _rb.velocity = Vector2.zero; // 停止移动
            UpdateAnimationState(0);     // 更新为静止状态
            return;
        }
        // 处理移动（物理相关逻辑在FixedUpdate中执行）
        MoveCharacter();
    }

    private void MoveCharacter()
    {
        // 计算移动速度
        Vector2 movement = new Vector2(horizontalInput * moveSpeed, _rb.velocity.y);
        _rb.velocity = movement;

        // 获取水平移动速度的绝对值
        float horizontalSpeed = Mathf.Abs(_rb.velocity.x);

        // 更新动画状态
        UpdateAnimationState(horizontalSpeed);

        // 处理转向（翻转精灵）
        if (spriteRenderer != null && horizontalInput != 0)
        {
            spriteRenderer.flipX = horizontalInput < 0;
        }
    }

    /// <summary> 更新动画状态
    /// </summary>
    /// <param name="speed"></param>
    private void UpdateAnimationState(float speed)
    {
        // 判断是否在移动
        isMoving = speed > 0.1f;

        // 更新动画参数
        _anim.SetFloat("Speed", speed / moveSpeed); // 归一化速度（0-1范围）
    }

    /// <summary> 检测输入是否锁定
    /// </summary>
    /// <param name="isDialogueActive"></param>
    private void OnDialogueStateChanged(bool isDialogueActive)
    {
        if (isDialogueActive)
        {
            // 禁用移动
            isMovementEnabled = false;
            UpdateAnimationState(0);
        }
        else
        {
            // 启用移动
            isMovementEnabled = true;
        }
    }

    /// <summary>
    /// 播放行走音效,动画事件调用
    /// </summary>
    private void PlaySFXAudio()
    {
        AudioManager.Instance.PlaySFX("SFX_PlayWalk");
    }

}
