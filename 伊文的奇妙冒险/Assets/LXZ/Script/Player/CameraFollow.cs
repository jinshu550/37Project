using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("跟随参数")]
    [Tooltip("摄像机与角色的偏移量（如角色在画面中央：(0,1, -10)）")]
    public Vector3 offset = new Vector3(0, 1, -10); // 3D偏移（z轴通常为-10，确保看到2D画面）
    [Tooltip("跟随平滑度（值越小越平滑，建议0.1-0.3）")]
    public float smoothSpeed = 0.125f;

    [Header("边界限制")]
    public Collider2D boundary;//用碰撞体来定义边界

    private Camera mainCamera;
    private Transform target; // 最终跟随的角色
    private Bounds boundaryBounds; // 边界碰撞体的包围盒

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        // 如果碰撞体发生变化，更新边界
        if (boundary != null)
        {
            boundaryBounds = boundary.bounds;
        }
    }

    // 用LateUpdate确保在角色移动后再更新摄像机，避免抖动
    private void LateUpdate()
    {
        //自动获取角色
        if (target == null && GameManager.Instance != null)
        {
            target = GameManager.Instance.GetCurrentPlayerTransform();
        }

        //没有角色时，不执行跟随逻辑（避免报错）
        if (target == null)
            return;

        // 计算目标位置（角色位置 + 偏移量）
        Vector3 desiredPosition = target.position + offset;
        // 平滑过渡到目标位置
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        //计算摄像机视口在世界空间中的边界
        float cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;
        float cameraHalfHeight = mainCamera.orthographicSize;

        //计算可移动的边界范围（考虑摄像机自身大小）
        float minX = boundaryBounds.min.x + cameraHalfWidth;
        float maxX = boundaryBounds.max.x - cameraHalfWidth;
        float minY = boundaryBounds.min.y + cameraHalfHeight;
        float maxY = boundaryBounds.max.y - cameraHalfHeight;

        // 限制摄像机在边界内（只限制x和y轴，z轴保持不变）
        float clampedX = Mathf.Clamp(smoothedPosition.x, minX, maxX);
        float clampedY = Mathf.Clamp(smoothedPosition.y, minY, maxY);

        // 应用最终位置
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }

    public static CameraFollow instance;

    private void OnEnable()
    {
        instance = this;
    }

}
