using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyViewCreator : Singleton<EnemyViewCreator>
{
    [SerializeField] private EnemyView enemyViewPrefab;
    public EnemyView CreateEnemyView(EnemyData enemyData, Vector3 position, Quaternion rotation)
    {
        EnemyView enemyView = Instantiate(enemyViewPrefab, position, rotation);
        enemyView.AdjustUILayerOrder();
        //enemyView.spriteRenderer.transform.localScale = new Vector3(0.2f, 0.2f);
        enemyView.SetUp(enemyData);
        return enemyView;
    }

}
