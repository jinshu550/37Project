using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartup : ExploreSingleton<GameStartup>
{
    //同一次游戏运行中，启动动画是否已播放
    public bool IsStartAnimationPlayed { get; private set; }
    protected override void Awake()
    {
        base.Awake();

        // 初始化：游戏刚启动时，动画未播放
        IsStartAnimationPlayed = false;
    }

    /// <summary> 标记启动动画已播放 </summary>
    public void MarkAnimationPlayed()
    {
        IsStartAnimationPlayed = true;
    }
}
