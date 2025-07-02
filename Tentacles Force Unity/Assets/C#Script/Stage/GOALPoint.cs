using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゴール地点の処理
/// </summary>
public class GOALPoint : MonoBehaviour
{
    // インスペクターから設定する変数
    [Header("現在のステージの番号を入力")]
    [SerializeField] private int UNLOCK_STAGE_NUMBER; // 解放するステージの番号

    [Header("ゴール時のSE")]
    [SerializeField] private string SE_NAME;           // SEの名前

    private void Start()
    {
        GameManager.Instance.currentGameState = GameState.Game; // ゲーム状態をゲームに設定
    }

    // オブジェクトすり抜け判定取得
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) // プレイヤータグに接触したら
        {
            if (StageManager.Instance != null && StageManager.Instance.IsStageUnlocked(UNLOCK_STAGE_NUMBER) == false)
            {
                StageManager.Instance.UnlockStage(UNLOCK_STAGE_NUMBER); // ステージを解放
            }

            if (SE_NAME != null)
            {
                SoundManager.Instance.PlaySE(SE_NAME); // SEを再生
            }

            GameManager.Instance.CurrentGameState = GameState.Clear; // ゲーム状態をクリアに設定
            // SceneChangeManager.Instance.ChangeSceneLoad(SCENE_CHANGE_NAME, SCENE_CHANGE_TIME); // 一定時間待機後シーン遷移
        }
    }
}
