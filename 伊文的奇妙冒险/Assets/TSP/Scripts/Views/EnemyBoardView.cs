using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyBoardView : MonoBehaviour
{
    [SerializeField] private List<Transform> slots;
    public List<EnemyView> EnemyViews { get; private set; } = new();
    public void AddEnemy(EnemyData enemyData)
    {
        Transform slot = slots[EnemyViews.Count];
        //实例化伤害UI
        EnemyView enemyViews = EnemyViewCreator.Instance.CreateEnemyView(enemyData, slot.position, slot.rotation);
        enemyViews.transform.parent = slot;
        EnemyViews.Add(enemyViews);
    }
    public IEnumerator RemoveEnemy(EnemyView enemyView)
    {
        EnemyViews.Remove(enemyView);
        Tween tween = enemyView.transform.DOScale(Vector3.zero, 0.25f);
        yield return tween.WaitForCompletion();
        Destroy(enemyView.gameObject);
    }
}
