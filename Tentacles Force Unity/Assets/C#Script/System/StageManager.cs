using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージの進行状態管理をするシングルトン
/// </summary>
public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    [SerializeField] private int totalStages = 4; // ステージ数

    private List<bool> stageUnlocked; // ステージ解放状況のリスト

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンロード後も保持
        }
        else
        {
            Destroy(gameObject); // 同じものがあれば消去
        }
        LoadStageData(); // ステージの解放状態をロード
    }

    private void Start()
    {
        
    }

    // 指定された数値のステージが解放されているか判定
    public bool IsStageUnlocked(int stageIndex)
    {
        if (stageUnlocked == null || stageIndex >= stageUnlocked.Count)
        {
            // stageUnlockedがnullまたはインデックスが範囲外の場合のエラーハンドリング
            Debug.LogError("stageUnlocked is not initialized or index is out of range!");
            return false; // エラー回避
        }
        return stageUnlocked[stageIndex]; // ステージが解放されているか判定
    }

    // 指定された数値のステージを解放する
    public void UnlockStage(int stageIndex)
    {
        if (stageIndex < stageUnlocked.Count && !stageUnlocked[stageIndex])
        {
            stageUnlocked[stageIndex] = true; // ステージを解放
            SaveStageData(); // 解放状況をセーブ
        }
    }

    // PlayerPrefsに解放状況をセーブ
    private void SaveStageData()
    {
        for (int i = 0; i < stageUnlocked.Count; i++)
        {
            PlayerPrefs.SetInt($"Stage_{i}_Unlocked", stageUnlocked[i] ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    // PlayerPrefsからデータをロードする
    private void LoadStageData()
    {
        // 新たなリストを作成
        stageUnlocked = new List<bool>();
        for (int i = 0; i < totalStages; i++)
        {
            int value = PlayerPrefs.GetInt($"Stage_{i}_Unlocked", i == 0 ? 1 : 0); // 初期値；ステージ1のみ解放
            stageUnlocked.Add(value == 1);
        }
    }

    // PlayerPrefs内のデータをリセットする
    private void DeleteStageDate()
    {
        for (int i = 0; i < totalStages; i++)
        {
            PlayerPrefs.DeleteKey($"Stage_{i}_Unlocked");
        }
        LoadStageData(); // 初期状態を再ロード
    }
}
