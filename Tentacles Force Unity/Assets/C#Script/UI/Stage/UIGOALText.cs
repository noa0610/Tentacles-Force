using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/*ゴール時にUIを表示するクラス
 * 
 * ゲーム終了のタイミングで指定したオブジェクトをアクティブ化させます。
 * ゴール直後にアクティブ化、フェードインするUIを指定できます。
 * 指定した時間後にアクティブ化、フェードインするUIを指定できます。
 */
public class UIGOALText : MonoBehaviour
{
    // インスペクターから設定する変数
    [Header("ゴール直後にアクティブ化するオブジェクト")]
    [SerializeField] private GameObject FirstActive;

    [Header("ゴール直後にフェードイン表示するUI")]
    [SerializeField] private Graphic[] FirstFadeIn;


    [Header("指定時間後にアクティブ化するオブジェクト")]
    [SerializeField] private GameObject NextActive;

    [Header("指定時間後にフェードイン表示するUI")]
    [SerializeField] private Graphic[] NextFadeIn;


    [Header("クリアタイム表示")]
    [SerializeField] private TextMeshProUGUI CLEARTIME_TEXT; // クリアタイム表示のテキスト
    [SerializeField] private GameObject TIMEROBJECT; // 時間を計測するスクリプトのオブジェクト

    [Header("表示設定")]
    [SerializeField] private float NextActiveTime = 2; // ウィンドウ表示までの時間
    [SerializeField] private float FadeInTime = 1; // フェード効果を反映させる時間

    // [Header("ゴール時のSE")]
    // [SerializeField] private string SE_NAME; // SEの名前


    // 内部処理する変数
    private UITimerText _uITimerText; // 時間計測タイマー参照用

    private void Start()
    {
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
        
        StartCoroutine(GOALActive()); // ゴールを確認するコルーチンを開始
    }

    // ゴールを確認したらUIを表示
    private IEnumerator GOALActive()
    {
        while (GameManager.Instance.CurrentGameState != GameState.Clear) // ゴールするまで繰り返す
        {
            yield return null; // 次のフレームまで待機
        }

        // ゴールした瞬間最初にゴールのテキストを表示
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

        // 最初のゴールのテキストを非表示
        FirstActive.SetActive(false);

        // ゴール後に表示するUIをフェードイン
        NextActive.SetActive(true);
        if (NextFadeIn != null)
        {
            FadeInGraphics(NextFadeIn, FadeInTime);
        }


        // クリアタイムのテキストを表示
        if (CLEARTIME_TEXT != null && TIMEROBJECT != null)
        {
            _uITimerText = TIMEROBJECT.GetComponent<UITimerText>();
            if (_uITimerText != null)
            {
                TimeSpan timeSpan = TimeSpan.FromSeconds(_uITimerText.CountTimer);
                CLEARTIME_TEXT.text = string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
                StartCoroutine(UIFade.FadeIn(CLEARTIME_TEXT, 1));
            }
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
