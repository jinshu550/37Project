using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingPanelManager : MonoBehaviour
{
    [Header("音频设置 - BGM")]
    [SerializeField] private Slider bgmSlider;          // BGM音量滑块（0-100）

    [Header("音频设置 - SFX")]
    [SerializeField] private Slider sfxSlider;          // 音效音量滑块（0-100）

    [Header("音频混合器")]
    [SerializeField] private AudioMixer mainMixer;      // 主音频混合器
    [SerializeField] private string bgmMixerParam = "BGM";  // BGM混合器参数名
    [SerializeField] private string sfxMixerParam = "SFX";  // SFX混合器参数名

    [Header("亮度设置")]
    [SerializeField] private Slider lightSlider;  // 亮度滑块（0-100）
    private float minLightIntensity => GameManager.DefaultMinLightIntensity;
    private float maxLightIntensity => GameManager.DefaultMaxLightIntensity;
    private Light2D currentSceneLight2D;

    private void Awake()
    {
        // 初始化滑块
        BindSliderEvents();
        if (GameManager.Instance.currentSceneLight2D != null)
        {
            currentSceneLight2D = GameManager.Instance.currentSceneLight2D;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        // 面板激活时，同步“已保存的设置”到UI控件
        SyncSavedSettingsToUI();
    }
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        // 切换场景后，重新获取当前场景的Light2D
        if (GameManager.Instance?.currentSceneLight2D != null)
        {
            currentSceneLight2D = GameManager.Instance?.currentSceneLight2D;
        }
    }

    /// <summary>场景切换时做出改变</summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SyncSavedSettingsToUI();
    }

    /// <summary>将保存的设置同步到UI控件</summary>
    private void SyncSavedSettingsToUI()
    {
        var (bgmVol, sfxVol) = GameDataManager.Instance.GetSavedVolumes();

        // BGM音量
        bgmSlider?.SetValueWithoutNotify(bgmVol);
        EventHandler.CallOnGroupVolumeChanged(bgmMixerParam, bgmVol);

        // SFX音量
        sfxSlider?.SetValueWithoutNotify(sfxVol);
        EventHandler.CallOnGroupVolumeChanged(sfxMixerParam, sfxVol);

        // 亮度
        int brightness = GameDataManager.Instance.GetSavedBrightness();
        lightSlider?.SetValueWithoutNotify(brightness);
        OnSliderValueChanged(brightness);
    }

    /// <summary>绑定滑块与事件的关联</summary>
    private void BindSliderEvents()
    {
        bgmSlider?.onValueChanged.AddListener(OnBgmSliderChanged);
        sfxSlider?.onValueChanged.AddListener(OnSfxSliderChanged);
        lightSlider?.onValueChanged.AddListener(OnSliderValueChanged);

        // 初始化滑块范围
        bgmSlider.minValue = sfxSlider.minValue = lightSlider.minValue = 0;
        bgmSlider.maxValue = sfxSlider.maxValue = lightSlider.maxValue = 100;
    }

    #region 音量调节逻辑
    private void OnBgmSliderChanged(float value)
    {
        // 计算当前BGM和SFX的音量
        float currentBgmVolume = value; // 滑块0-100转成0-1范围
        float currentSfxVolume = sfxSlider.value;

        // 用GameDataManager统一保存
        GameDataManager.Instance.SaveVolumeSettings(currentBgmVolume, currentSfxVolume);
        EventHandler.CallOnGroupVolumeChanged(bgmMixerParam, currentBgmVolume);
    }

    private void OnSfxSliderChanged(float value)
    {
        // 计算当前SFX和BGM的音量（0-1范围）
        float currentSfxVolume = value;
        float currentBgmVolume = bgmSlider.value;
        
        //用GameDataManager统一保存
        GameDataManager.Instance.SaveVolumeSettings(currentBgmVolume, currentSfxVolume);
        EventHandler.CallOnGroupVolumeChanged(sfxMixerParam, currentSfxVolume);
    }


    #endregion

    #region 亮度调节逻辑
    /// <summary> 光照同步 滑块
    /// </summary>
    /// <param name="value"></param>
    private void OnSliderValueChanged(float value)
    {
        if (currentSceneLight2D == null)
        {
            return;
        }

        //目标强度=最小强度+滑块的百分比*（光照强度范围）
        float targetIntensity = minLightIntensity + value / lightSlider.maxValue * (maxLightIntensity - minLightIntensity);

        //赋值  并进行一个数值保护
        currentSceneLight2D.intensity = Mathf.Clamp(targetIntensity, minLightIntensity, maxLightIntensity);

        int brightnessValue = Mathf.RoundToInt(value); // 滑块数值存储
        GameDataManager.Instance.SaveBrightness(brightnessValue);
    }

    #endregion


}