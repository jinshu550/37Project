using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ShieldEffectSetup : MonoBehaviour
{
    [Header("粒子预制体")]
    public ParticleSystem shieldParticlePrefab; // 单个盾牌粒子预制体

    [Header("运动参数")]
    public float orbitRadius = 1.0f; // 环绕半径（相对于父对象）
    public float orbitSpeed = 60f; // 环绕速度（度/秒）
    public bool clockwise = true; // 顺时针旋转

    [Header("布局参数")]
    public int shieldCount = 3; // 盾牌数量

    [Header("层级设置")]
    public Transform characterTransform; // 人物Transform，用于判断前后
    public int frontLayerOrder = 10; // 盾牌在人物前方时的层级
    public int behindLayerOrder = 5; // 盾牌在人物后方时的层级

    private ParticleSystem[] shields; // 存储实例化的盾牌

    private bool isInitialized = false;
    private float currentAngle = 0f; // 当前旋转角度

    // 实例化后自动初始化
    private void OnEnable()
    {
        if (!isInitialized)
        {
            Initialize();
        }
        else
        {
            // 重新激活时重启粒子
            if (shields != null)
            {
                foreach (var shield in shields)
                {
                    if (shield != null) shield.Play();
                }
            }
        }
    }

    // 初始化系统
    private void Initialize()
    {
        // 实例化盾牌粒子
        SpawnShields();
        isInitialized = true;
    }

    // 实例化所有盾牌粒子
    private void SpawnShields()
    {
        if (shieldParticlePrefab == null)
        {
            Debug.LogError("请赋值盾牌粒子预制体！");
            return;
        }

        shields = new ParticleSystem[shieldCount];
        float angleStep = 360f / shieldCount;

        for (int i = 0; i < shieldCount; i++)
        {
            // 实例化粒子，作为当前父对象的子物体
            ParticleSystem shield = Instantiate(
                shieldParticlePrefab,
                transform.position,
                Quaternion.identity,
                transform
            );

            shield.gameObject.name = $"Shield_{i + 1}";
            shields[i] = shield;

            // 初始位置设置（围绕Y轴均匀分布）
            float angle = i * angleStep * Mathf.Deg2Rad;
            UpdateShieldPosition(shield.transform, angle);

            shield.Play();
        }
    }

    // 更新单个盾牌的位置
    private void UpdateShieldPosition(Transform shieldTransform, float baseAngle)
    {
        // 计算实际角度 = 基础角度 + 当前旋转角度
        float actualAngle = baseAngle + currentAngle;

        // 计算XZ平面上的位置（围绕Y轴旋转）
        float x = Mathf.Cos(actualAngle) * orbitRadius;
        float z = Mathf.Sin(actualAngle) * orbitRadius;

        // 设置位置（Y轴保持与父对象一致）
        shieldTransform.localPosition = new Vector3(x, 0, z);

        // 让盾牌始终面向玩家
        shieldTransform.LookAt(transform.position);
    }

    // 每帧更新环绕位置
    private void Update()
    {
        if (!isInitialized || shields == null) return;

        // 更新当前旋转角度
        float direction = clockwise ? -1 : 1;
        currentAngle += direction * orbitSpeed * Mathf.Deg2Rad * Time.deltaTime;

        // 确保角度在合理范围内
        currentAngle %= (2 * Mathf.PI);

        // 更新每个盾牌的位置
        float angleStep = 2 * Mathf.PI / shieldCount;
        for (int i = 0; i < shields.Length; i++)
        {
            if (shields[i] != null)
            {
                UpdateShieldPosition(shields[i].transform, i * angleStep);
            }
        }
    }
}