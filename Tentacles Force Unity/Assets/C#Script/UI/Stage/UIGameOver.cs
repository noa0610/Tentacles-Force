using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲームオーバー時にUIを表示するクラス
/// </summary>
public class UIGameOver : MonoBehaviour
{
    // インスペクターから設定する変数
    [Header("ゲームオーバー直後にアクティブ化するオブジェクト")]
    [SerializeField] private GameObject FirstActive;

    [Header("ゲームオーバー直後にフェードイン表示するUI")]
    [SerializeField] private Graphic[] FirstFadeIn;


    [Header("指定時間後にアクティブ化するオブジェクト")]
    [SerializeField] private GameObject NextActive;

    [Header("指定時間後にフェードイン表示するUI")]
    [SerializeField] private Graphic[] NextFadeIn;

    [Header("表示設定")]
    [SerializeField] private float NextActiveTime = 2; // ウィンドウ表示までの時間
    [SerializeField] private float FadeInTime = 1; // フェード効果を反映させる時間

    // [Header("ゲームオーバー時のSE")]
    // [SerializeField] private string SE_NAME; // SEの名前

    private void Start()
    {
        Time.timeScale = 1f;

        // 初期化の際、設定忘れ防止でオブジェクトを非表示にする
        if (FirstActive != null)
        {
            FirstActive.SetActive(false);
        }
        else
        {
            Debug.LogWarning("GOAL_TEXT_OBJECT is not assigned in UIGOALText script.");
        }

        if (NextActive != null)
        {
            NextActive.SetActive(false);
        }
        else
        {
            Debug.LogWarning("GOAL_WINDOW_OBJECT is not assigned in UIGOALText script.");
        }
        

        StartCoroutine(GameOverUIActive());
    }

    private IEnumerator GameOverUIActive()
    {
        while (GameManager.Instance.CurrentGameState != GameState.GameOver) // ゲームオーバーまで繰り返す
        {
            yield return null; // 次のフレームまで待機
        }

        // ゲームオーバー直後にテキストを表示
        FirstActive.SetActive(true);
        if (FirstFadeIn != null)
        {
            FadeInGraphics(FirstFadeIn, FadeInTime);
        }

        // if (SE_NAME != null)
        // {
        //     SoundManager.Instance.PlaySE(SE_NAME); // SEを再生
        // }

        // ウィンドウ表示までの時間待機
        yield return new WaitForSeconds(NextActiveTime);

        // 最初のゲームオーバーのテキストを非表示
        FirstActive.SetActive(false);

        // ゴール後に表示するUIをフェードイン
        NextActive.SetActive(true);
        if (NextFadeIn != null)
        {
            FadeInGraphics(NextFadeIn, FadeInTime);
        }

        // フェードインが終わるまでの時間待機
        yield return new WaitForSeconds(FadeInTime);

        // ゲーム時間を停止
        Time.timeScale = 0f;
    }

    // フェードイン処理を一括適用
    private void FadeInGraphics(Graphic[] graphics, float duration)
    {
        foreach (var graphic in graphics)
        {
            if (graphic != null)
            {
                StartCoroutine(UIFade.FadeIn(graphic, duration));
            }
        }
    }
}
