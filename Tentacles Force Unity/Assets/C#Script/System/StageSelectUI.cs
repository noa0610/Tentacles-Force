using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// ステージセレクト画面のボタンをキーボードで選択するスクリプト
/// </summary>
public class StageSelectUI : MonoBehaviour
{
    // インスペクターから設定する変数
    [Header("各ステージのボタン")]
    [SerializeField] private Button[] STAGE_BUTTUNS; // 各ステージのボタン

    [Header("各ステージのシーン名")]
    [SerializeField] private string[] SCENE_NAME; // 遷移するシーンの名前

    [Header("ボタン選択のSE")]
    [SerializeField] private string SE_CHANGE_NAME;   // 選択のSEの名前

    [Header("ボタン決定のSE")]
    [SerializeField] private string SE_SELECT_NAME;   // 決定のSEの名前

    // 内部処理する変数
    private int selectedStageIndex = 0; // 現在選択中のステージ

    private void Start()
    {
        if (StageManager.Instance == null)
        {
            Debug.LogError("StageManager.Instance is not initialized!");
            return; // エラー回避
        }
        UpdateButtonStates(); // ステージ解放状況に基づいてボタンの有効/無効を設定
        HighlightSelectedButton(); // 最初のステージボタンをハイライト（選択状態）
    }

    private void Update()
    {
        HandleKeyboardInput(); // キーボード操作を処理
    }

    // 解放されたステージだけボタンを有効化
    private void UpdateButtonStates()
    {
        for (int i = 0; i < STAGE_BUTTUNS.Length; i++)
        {
            if (StageManager.Instance.IsStageUnlocked(i))
            {
                STAGE_BUTTUNS[i].interactable = true; // 解放されていれば有効化
                int stageIndex = i; // ローカル変数にキャプチャ
                STAGE_BUTTUNS[i].onClick.AddListener(() => SelectStage(stageIndex));
            }
            else
            {
                STAGE_BUTTUNS[i].interactable = false; // 解放されていなければ無効化
            }
        }
    }

    // 選択しているボタンをハイライト表示
    private void HighlightSelectedButton()
    {
        for (int i = 0; i < STAGE_BUTTUNS.Length; i++)
        {
            ColorBlock colorBlock = STAGE_BUTTUNS[i].colors; // ボタンの色を取得
            colorBlock.normalColor = (i == selectedStageIndex) ? Color.yellow : Color.white; // 現在選択したものは黄色、それ以外の選択肢は白
            STAGE_BUTTUNS[i].colors = colorBlock; // ハイライトを適用
        }
    }

    // ステージ選択操作
    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) // 左キーで次のステージ
        {
            // 現在のインデックスから左方向に次の解放済みステージを探す
            do
            {
                selectedStageIndex--;
                if (selectedStageIndex < 0) // 最初のステージを超えたら最後に戻る
                {
                    selectedStageIndex = STAGE_BUTTUNS.Length - 1;
                }
            }
            while (!StageManager.Instance.IsStageUnlocked(selectedStageIndex)); // 解放されているまで繰り返す

            if (SE_CHANGE_NAME != null)
            {
                SoundManager.Instance.PlaySE(SE_CHANGE_NAME); // SEを再生
            }

            HighlightSelectedButton(); // 選択したボタンをハイライト表示

            Debug.Log($"{selectedStageIndex}");
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) // 右キーで前のステージ
        {
            // 現在のインデックスから右方向に次の解放済みステージを探す
            do
            {
                selectedStageIndex++;
                if (selectedStageIndex >= STAGE_BUTTUNS.Length) // 最後のステージを超えたら最初に戻る
                {
                    selectedStageIndex = 0;
                }
            }
            while (!StageManager.Instance.IsStageUnlocked(selectedStageIndex)); // 解放されているまで繰り返す

            if (SE_CHANGE_NAME != null)
            {
                SoundManager.Instance.PlaySE(SE_CHANGE_NAME); // SEを再生
            }

            HighlightSelectedButton(); // 選択したボタンをハイライト表示

            Debug.Log($"{selectedStageIndex}");
        }

        if (Input.GetKeyDown(KeyCode.Return) && StageManager.Instance.IsStageUnlocked(selectedStageIndex))
        {
            if (StageManager.Instance.IsStageUnlocked(selectedStageIndex)) // 解放済みなら選択可能
            {
                if (SE_SELECT_NAME != null)
                {
                    SoundManager.Instance.PlaySE(SE_SELECT_NAME); // SEを再生
                }
                
                SelectStage(selectedStageIndex);
            }

            Debug.Log($"{selectedStageIndex}");
        }
    }

    // ステージを選択したときの処理
    private void SelectStage(int stageIndex)
    {
        Debug.Log($"Stage {stageIndex + 1} selected!");
        // ステージ開始の処理を追加（例: SceneManager.LoadScene）
        SceneManager.LoadScene($"{SCENE_NAME[stageIndex]}");
    }
}
