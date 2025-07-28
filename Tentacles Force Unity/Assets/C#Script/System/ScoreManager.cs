using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/** スコア管理
  * 
  * 現在のスコアを保持・公開するObserver
  */
public class ScoreManager : MonoBehaviour
{
    // 値の変更を通知できるリアクティブな変数
    private ReactiveProperty<int> _score = new ReactiveProperty<int>(0);
    // 他クラスからスコアを購読できるプロパティ
    public IReadOnlyReactiveProperty<int> Score => _score;

    private void Start()
    {
        MessageBroker.Default
            .Receive<EnemyDefeatedMessage>()
            .Subscribe(msg =>
            {
                _score.Value += msg.ScoreValue;
                Debug.Log($"スコア加算: +{msg.ScoreValue} 現在のスコア: {_score.Value}");
            })
            .AddTo(this);
    }

    // public void RegisterEnemy(EnemyMove enemy)
    // {
    //     enemy.OnDefeated
    //         .Subscribe(_ =>
    //         {
    //             _score.Value += enemy.scoreValue;
    //             Debug.Log("スコア加算！ 現在のスコア: " + _score.Value);
    //         })
    //         .AddTo(this); // 破棄管理

    //     // Enemyに直接アクセスしていない、イベントのみ受け取る
    // }
}
