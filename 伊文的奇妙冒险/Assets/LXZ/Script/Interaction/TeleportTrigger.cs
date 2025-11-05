using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class TeleportTrigger : MonoBehaviour
{
    [Header("传送目标设置")]
    [Tooltip("传送目标位置")]
    public Transform teleportDestination;
    [Tooltip("传送后相机应使用的新边界碰撞体")]
    public Collider2D newCameraBoundary;
    [Tooltip("传送后运用的新背景音乐")]
    [SerializeField] private string newBGMName = "BGM_Scene2_2";

    [SerializeField] private KeyCode key;

    [Header("触发设置")]
    [SerializeField] private bool ignoreSelf = true; //是否需要忽略触发器自身的碰撞
    [SerializeField] private Collider2D triggerCollider;
    [SerializeField] private CameraFollow cameraFollow;

    void Awake()
    {
        if (triggerCollider == null)
        {
            triggerCollider = GetComponent<Collider2D>();
        }
        // 查找场景中的摄像机跟随组件
        if (cameraFollow == null)
            cameraFollow = FindObjectOfType<CameraFollow>();
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        // 忽略自身碰撞（如果需要）
        if (ignoreSelf && other.transform == transform)
        {
            return;
        }


        // 检查是否是玩家（假设玩家有特定标签）

        if (other.CompareTag("Player") && Input.GetKey(key))
        {
            TeleportObject(other.transform);
        }
    }

    /// <summary>
    /// 传送对象到目标位置
    /// </summary>
    private void TeleportObject(Transform target)
    {
        if (teleportDestination == null)
        {
            Debug.LogError("传送目标未设置！", this);
            return;
        }

        //转场音效
        AudioManager.Instance.PlayBGM(newBGMName);

        // 执行传送
        target.position = teleportDestination.position;

        // 通知摄像机立即跟随到新位置
        if (cameraFollow != null)
        {
            // 如果设置了新的相机边界，则更新
            if (newCameraBoundary != null)
            {
                // 如果设置了新的相机边界，则更新（直接访问public字段，无需反射）
                if (newCameraBoundary != null)
                {
                    cameraFollow.boundary = newCameraBoundary; // 直接赋值public字段
                }
            }

            // 计算摄像机应该在的新位置
            Vector3 newCameraPosition = target.position + cameraFollow.offset;

            // 直接设置摄像机位置（不使用平滑过渡）
            cameraFollow.transform.position = new Vector3(
                newCameraPosition.x,
                newCameraPosition.y,
                cameraFollow.transform.position.z
            );
        }
    }
}
