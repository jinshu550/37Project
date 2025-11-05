using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadBack : MonoBehaviour
{
    [Header("播放的UI")]
    [SerializeField] private GameObject firstCinematic;//挂载视频动画的UI
    [SerializeField] private GameObject LoadingUI;//挂载视频动画的UI

    private int playCount = 0; //检查播放视频的动画
    private string targetScene;  //需要跳转的场景

    private VideoFadeController videoController;
    //异步加载场景的句柄
    private AsyncOperation asyncLoadOperation;

    void Start()
    {
        //读取检查 判断第几次播放动画视频
        playCount = GameDataManager.Instance.GetCinematicPlayCount();

        targetScene = Loader.ReturnToTargetScene();
        //安全获取动画控制器
        if (firstCinematic != null)
        {
            videoController = firstCinematic.GetComponent<VideoFadeController>();
            // 绑定动画结束回调
            if (videoController != null)
            {
                EventHandler.OnVideoComplete += HandleVideoEnd;
            }
        }
        LoadTargetScenePlay();
    }
    void OnDestroy()
    {
        if (videoController != null)
        {
            EventHandler.OnVideoComplete -= HandleVideoEnd;
        }
    }

    public void LoadTargetScenePlay()
    {
        switch (targetScene)
        {
            case "Explore_001Scene":
                if (playCount == 0)
                {
                    //先启动异步加载，再播放视频
                    asyncLoadOperation = SceneManager.LoadSceneAsync(targetScene);
                    asyncLoadOperation.allowSceneActivation = false; // 暂时不激活场景

                    if (firstCinematic != null && !firstCinematic.activeSelf)
                    {
                        firstCinematic.SetActive(true);
                    }
                }
                else // 非首次：直接跳场景
                {
                    //切换背景音乐
                    AudioManager.Instance.PlayBGM("BGM_Scene1");
                    SceneManager.LoadScene(targetScene);
                }
                break;
            case "Explore_002Scene":
                AudioManager.Instance.PlayBGM("BGM_Scene2_1");
                SceneManager.LoadScene(targetScene);
                break;
            case "Fight_Scene":
                AudioManager.Instance.PlayBGM("BGM_FightScene");
                SceneManager.LoadScene(targetScene);
                break;
            default:
                //LoadingUI.SetActive(true);
                AudioManager.Instance.PlayBGM("BGM_MainMenu");
                SceneManager.LoadScene(targetScene);
                break;
        }
    }

    /// <summary> 动画结束回调 </summary>
    private void HandleVideoEnd()
    {
        // 更新动画计数（标记为“已播放过”，下次不再触发）
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.IncrementCinematicCount();
        }
        // 跳转目标场景
        //SceneManager.LoadScene(targetScene);
        // 首次播放且异步加载了场景时，激活场景
        if (playCount == 0 && asyncLoadOperation != null)
        {
            asyncLoadOperation.allowSceneActivation = true; // 立即跳转
            AudioManager.Instance.PlayBGM("BGM_Scene1");
        }
    }


}
