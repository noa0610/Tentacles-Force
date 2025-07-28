using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** 敵生成機能
  * 
  * 円形範囲内の敵を検出、接近を感知し、
  * 敵キャラクターを生成する機能
  */
public class EnemySpwner : MonoBehaviour
{
    [Tooltip("スコア管理オブザーバー。")]
    [SerializeField] private ScoreManager scoreManager;

    [Header("スポーン設定")]
    [Tooltip("インスタンス化を行う敵キャラクターのプレハブ。")]
    [SerializeField] private GameObject enemyPrefab;

    [Tooltip("敵が出現するプレイヤーからの距離。")]
    [SerializeField] private float _spawnRadius = 10f;

    [Tooltip("trueの場合、最初にスポーンした後はアタッチされたオブジェクトを破棄します。")]
    [SerializeField] private bool _spawnOnce = true;

    [Header("参照オブジェクトタグ")]
    [Tooltip("距離を参照するオブジェクトのタグ。")]
    [SerializeField] private string TargetTag;

    [Header("検出範囲")]
    [Tooltip("参照を行う半径範囲")]
    [SerializeField] private float _playerSearchRadius = 30f;

    private Transform _player;
    private bool _hasSpawned;

    private void Update()
    {
        if (_hasSpawned && _spawnOnce) return;

        if (_player == null)
        {
            _player = FindPlayerNearby();
            if (_player == null) return;
        }

        float distance = Vector2.Distance(_player.position, transform.position);
        if (distance <= _spawnRadius)
        {
            SpawnEnemy();
        }
    }

    private Transform FindPlayerNearby()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _playerSearchRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag(TargetTag))
            {
                return hit.transform;
            }
        }
        return null;
    }



    public void SpawnEnemy()
    {
        var enemyObj = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        _hasSpawned = true;
        // var enemy = enemyObj.GetComponent<EnemyMove>();
        // scoreManager.RegisterEnemy(enemy); // 生成したEnemyをScoreManagerに登録
        if (_spawnOnce)
        {
            Destroy(gameObject);
        }
    }

#if UNITY_EDITOR
    // シーンビューでスポーン参照範囲を視覚化
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _spawnRadius);

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, _playerSearchRadius);
    }
#endif
}
