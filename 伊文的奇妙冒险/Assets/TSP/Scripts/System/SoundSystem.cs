using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSystem : Singleton<SoundSystem>
{
    void OnEnable()
    {
        ActionSystem.AttachPerformer<SoundsPlayGA>(PlaySpecialSoundPerformer);
        ActionSystem.SubscribeReaction<DrawCardsGA>(DrawCardSoundReaction, ReactionTiming.POST);
        ActionSystem.SubscribeReaction<PlayCardGA>(PlayCardSoundReaction, ReactionTiming.POST);
    }
    void OnDisable()
    {
        ActionSystem.UnsubscribeReaction<DrawCardsGA>(DrawCardSoundReaction, ReactionTiming.POST);
        ActionSystem.UnsubscribeReaction<PlayCardGA>(PlayCardSoundReaction, ReactionTiming.POST);
        ActionSystem.DetachPerformer<SoundsPlayGA>();
    }
    public void SetUp()
    {
        AudioManager.Instance.PlayBGM("BGM_FightScene");
    }
    private void PlayCardSoundReaction(PlayCardGA playCardGA)
    {
        AudioManager.Instance.PlaySFX("SFX_CardPlace");
    }
    private void DrawCardSoundReaction(DrawCardsGA drawCardsGA)
    {
        StartCoroutine(PlayDrawCardSoundsCoroutine(drawCardsGA.Amount, "SFX_draw"));
    }
    public void PlaySound(string name)
    {
        AudioManager.Instance.PlaySFX(name);
    }
    private IEnumerator PlaySpecialSoundPerformer(SoundsPlayGA soundsPlayGA)
    {
        AudioManager.Instance.PlaySFX("这里是其他游戏动作（治疗 攻击 出牌等）的音效");
        yield return null;
    }
    private IEnumerator PlayDrawCardSoundsCoroutine(int playCount, string name)
    {
        // 音效间隔时间（可根据需求调整，0.1-0.2秒比较自然，太快会重叠，太慢会脱节）
        float interval = 0.15f;

        for (int i = 0; i < playCount; i++)
        {
            // 播放单次抽牌音效
            AudioManager.Instance.PlaySFX(name);

            // 等待间隔时间（最后一次播放后不需要等，避免多等一次）
            if (i < playCount - 1)
            {
                yield return new WaitForSeconds(interval);
            }
        }
    }
}
