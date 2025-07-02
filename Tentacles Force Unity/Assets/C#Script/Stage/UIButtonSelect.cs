using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// ボタンを選択してシーンを切り替えるUI用スクリプト
/// </summary>
public class UIButtonSelect : MonoBehaviour
{
    // インスペクターから設定する変数
    [Header("選択をするボタン")]
    [SerializeField] private Button[] SELECT_BUTTUNS; // 各ボタン

    [Header("遷移するシーンの名前")]
    [SerializeField] private string[] SCENE_NAME;     // 遷移するシーンの名前

    [Header("ボタン選択のSE")]
    [SerializeField] private string SE_CHANGE_NAME;   // 選択のSEの名前

    [Header("ボタン決定のSE")]
    [SerializeField] private string SE_SELECT_NAME;   // 決定のSEの名前

    // 内部処理する変数
    private int selectedButtunIndex = 0; // 現在選択中のボタン
    private void Start()
    {
        if (SELECT_BUTTUNS == null || SELECT_BUTTUNS.Length == 0)
        {
            Debug.LogError("SELECT_BUTTUNSが設定されていません。");
        }

        if (SCENE_NAME == null || SCENE_NAME.Length == 0)
        {
            Debug.LogError("SCENE_NAMEが設定されていません。");
        }
        else if (SELECT_BUTTUNS.Length != SCENE_NAME.Length)
        {
            Debug.LogError("SELECT_BUTTUNSとSCENE_NAMEの要素数が一致していません。");
        }
        
        HighlightSelectedButton(); // 最初のボタンをハイライト（選択状態）
    }

    private void Update()
    {
        HandleKeyboardInput(); // キーボード操作を処理
    }

    // 選択しているボタンをハイライト表示
    private void HighlightSelectedButton()
    {
        for (int i = 0; i < SELECT_BUTTUNS.Length; i++)
        {
            ColorBlock colorBlock = SELECT_BUTTUNS[i].colors; // ボタンの色を取得
            colorBlock.normalColor = (i == selectedButtunIndex) ? Color.yellow : Color.white; // 現在選択したものは黄色、それ以外の選択肢は白
            SELECT_BUTTUNS[i].colors = colorBlock; // ハイライトを適用
        }
    }

    // ステージ選択操作
    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) // 左キーで次のボタン
        {
            // 現在のインデックスから左方向のボタンを探す
            selectedButtunIndex--;
            if (selectedButtunIndex < 0) // 最初のボタンを超えたら最後に戻る
            {
                selectedButtunIndex = SELECT_BUTTUNS.Length - 1;
            }

            if (SE_CHANGE_NAME != null)
            {
                SoundManager.Instance.PlaySE(SE_CHANGE_NAME); // SEを再生
            }

            HighlightSelectedButton(); // 選択したボタンをハイライト表示

            Debug.Log($"{selectedButtunIndex}");
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) // 右キーで前のボタン
        {
            // 現在のインデックスから右方向のボタンを探す
            selectedButtunIndex++;
            if (selectedButtunIndex >= SELECT_BUTTUNS.Length) // 最後のボタンを超えたら最初に戻る
            {
                selectedButtunIndex = 0;
            }

            if (SE_CHANGE_NAME != null)
            {
                SoundManager.Instance.PlaySE(SE_CHANGE_NAME); // SEを再生
            }

            HighlightSelectedButton(); // 選択したボタンをハイライト表示

            Debug.Log($"{selectedButtunIndex}");
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (SE_SELECT_NAME != null)
            {
                SoundManager.Instance.PlaySE(SE_SELECT_NAME); // SEを再生
            }
            
            SelectButtun(selectedButtunIndex);

            Debug.Log($"{selectedButtunIndex}");
        }
    }

    // ボタンを選択したときの処理
    private void SelectButtun(int stageIndex)
    {
        Debug.Log($"ボタン {stageIndex} を選択: {SCENE_NAME[stageIndex]}");
        SceneChangeManager.Instance.ChangeSceneLoad(SCENE_NAME[stageIndex]); // シーン遷移する
    }
}
