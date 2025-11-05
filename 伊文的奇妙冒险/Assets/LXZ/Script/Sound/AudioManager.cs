using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

/// <summary>
/// 音频管理器，负责存储所有音频并提供播放和停止功能
/// </summary>
public class AudioManager : ExploreSingleton<AudioManager>
{
    // 可以在Inspector面板中添加多个音频配置
    public List<Sound> sounds;

    [Header("音频源")]
    private AudioSource m_BGM_Source;
    private AudioSource m_SFX_Source;

    // 存储音频名称 --> 音频源的映射
    private Dictionary<string, AudioClip> audiosDic;

    // 存储“音频名称 --> Sound配置”的映射
    private Dictionary<string, Sound> soundConfigDic;

    protected override void Awake()
    {
        base.Awake();
        // 初始化双音频源（挂载到AudioManager所在GameObject）
        InitFixedAudioSources();
        // 初始化字典（存储音频Clip和配置）
        audiosDic = new Dictionary<string, AudioClip>();
        soundConfigDic = new Dictionary<string, Sound>();
    }


    void Start()
    {
        // 填充音频字典 + 处理启动自动播放BGM
        FillAudioDictionaries();
        EventHandler.CallAudioManagerInited();//发送初始化完成事件
    }

    void OnEnable()
    {
        EventHandler.OnGroupVolumeChanged += OnGroupVolumeChanged;
    }

    private void OnDestroy()
    {
        EventHandler.OnGroupVolumeChanged -= OnGroupVolumeChanged;
    }

    #region 初始化逻辑
    /// <summary> 初始化BGM和SFX两个固定的音频源 </summary>
    private void InitFixedAudioSources()
    {
        // 初始化BGM音频源
        m_BGM_Source = gameObject.AddComponent<AudioSource>();
        m_BGM_Source.loop = true;          // BGM默认循环
        m_BGM_Source.priority = 128;       // 优先级低于SFX
        m_BGM_Source.playOnAwake = false;  // 不自动播放，手动控制

        // 初始化SFX音频源
        m_SFX_Source = gameObject.AddComponent<AudioSource>();
        m_SFX_Source.loop = false;         // SFX不循环
        m_SFX_Source.priority = 64;        // 音效优先级高于BGM
        m_SFX_Source.playOnAwake = false;

    }

    /// <summary>
    /// 填充音频字典（Clip和配置），并处理启动自动播放
    /// </summary>
    private void FillAudioDictionaries()
    {
        foreach (Sound sound in sounds)
        {
            // 跳过空Clip，避免报错
            if (sound.clip == null)
            {
                Debug.LogWarning($"音频[{sound.soundName}]的Clip为空，已跳过！");
                continue;
            }

            // 检查重复名称（确保每个soundName唯一）
            if (audiosDic.ContainsKey(sound.soundName))
            {
                Debug.LogWarning($"音频名称重复：[{sound.soundName}]，请修改唯一标识！");
                continue;
            }

            // 存入字典（用于快速查找）
            audiosDic.Add(sound.soundName, sound.clip);
            soundConfigDic.Add(sound.soundName, sound);

            // 处理启动自动播放（仅BGM）
            if (sound.playOnStart)
            {
                PlayBGM(sound.soundName);
            }
        }
    }

    #endregion

    #region 播放控制
    /// <summary>
    /// 根据名称播放音频
    /// </summary>
    /// <param name="soundName">音频名称</param>
    public void PlayBGM(string soundName)
    {
        // 检查字典中是否存在该BGM
        if (!audiosDic.TryGetValue(soundName, out AudioClip bgmClip) ||
            !soundConfigDic.TryGetValue(soundName, out Sound bgmConfig))
        {
            Debug.LogWarning($"BGM[{soundName}]不存在，请检查配置！");
            return;
        }

        // 检查是否为BGM分组
        if (bgmConfig.groupName != GroupName.bgm)
        {
            Debug.LogWarning($"音频[{soundName}]分组错误！当前为[{bgmConfig.groupName}]，需设置为[GroupName.bgm]才能通过PlaySFX播放");
            return;
        }

        // 切换BGM（如果正在播放相同BGM则跳过）
        if (m_BGM_Source.clip == bgmClip && m_BGM_Source.isPlaying)
            return;

        // 应用配置并播放
        m_BGM_Source.clip = bgmClip;
        m_BGM_Source.volume = bgmConfig.volume;
        m_BGM_Source.loop = bgmConfig.loop;
        m_BGM_Source.Play();
    }

    /// <summary>
    /// 根据名称停止音频
    /// </summary>
    /// <param name="soundName">音频名称</param>
    public void StopBGM(string soundName)
    {
        if (m_BGM_Source.isPlaying)
            m_BGM_Source.Stop();
    }

    /// <summary>
    /// 暂停/恢复当前BGM
    /// </summary>
    public void ToggleBGMPause()
    {
        if (m_BGM_Source.isPlaying)
            m_BGM_Source.Pause();
        else
            m_BGM_Source.UnPause();
    }


    /// <summary> 播放音效（单次播放）
    /// </summary>
    /// <param name="soundName">音效的标识名称（如"SFX_Click"）</param>
    public void PlaySFX(string soundName)
    {
        // 检查字典中是否存在该音效
        if (!audiosDic.TryGetValue(soundName, out AudioClip sfxClip) ||
            !soundConfigDic.TryGetValue(soundName, out Sound sfxConfig))
        {
            Debug.LogWarning($"音效[{soundName}]不存在，请检查配置！");
            return;
        }

        // 检查是否为SFX分组
        if (sfxConfig.groupName != GroupName.sfx)
        {
            Debug.LogWarning($"音频[{soundName}]分组错误！当前为[{sfxConfig.groupName}]，需设置为[GroupName.sfx]才能通过PlaySFX播放");
            return;
        }

        // 播放音效
        m_SFX_Source.clip = sfxClip;
        m_SFX_Source.PlayOneShot(sfxClip, sfxConfig.volume);
        m_SFX_Source.loop = false;
    }

    #endregion

    /// <summary> 调节音量 
    /// </summary>
    /// <param name="groupName">分组名称</param>
    /// <param name="volume">音量</param>
    private void OnGroupVolumeChanged(string groupName, float volume)
    {
        float normalizedVolume = Mathf.Clamp01(volume / 100f); // 限制音量在0-1范围

        if (groupName.ToLower() == "bgm")
        {
            m_BGM_Source.volume = normalizedVolume;
        }
        else if (groupName.ToLower() == "sfx")
        {
            m_SFX_Source.volume = normalizedVolume;
        }
        else
        {
            Debug.LogWarning($"未知的音量分组：[{groupName}]，仅支持'BGM'和'SFX'！");
        }
    }


    /// <summary>  存储单个音频的信息 </summary>
    [System.Serializable]
    public class Sound
    {
        [Header("音频标识")]
        public string soundName;

        [Header("音频剪辑")]
        public AudioClip clip;

        [Header("音频分组")]
        public GroupName groupName;

        [Header("音频音量")]
        [Range(0, 1)] public float volume = 0.7f;

        [Tooltip("是否循环播放")]
        public bool loop;

        [Tooltip("是否在游戏开始时自动播放")]
        public bool playOnStart;

    }

    public enum GroupName
    {
        bgm,
        sfx,
    }
}
