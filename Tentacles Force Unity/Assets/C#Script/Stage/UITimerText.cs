using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/* テキストにタイマーを表示するUIクラス
 * 
 * ゲーム開始のタイミングでアクティブ化され、時間の計測を開始します。
 * ゲーム終了のタイミングで非アクティブ化されます。
 */
public class UITimerText : MonoBehaviour
{
    // インスペクターから設定する変数
    [SerializeField] private GameObject OBJECT;
    [SerializeField] private TextMeshProUGUI TimerText; // テキストオブジェクト

    // 内部処理する変数
    private float countTimer; // 計測時間
    private TextMeshProUGUI timerText; // 計測時間を反映するテキスト
    private bool isTimerInitialized = false;

    // プロパティ
    public float CountTimer
    {
        get { return countTimer; }
    }

    public bool IsTimerInitialized
    {
        get { return isTimerInitialized; }
    }

    private void Start()
    {
        StartCoroutine(SetActiveTrueTimer());
    }

    private void Update()
    {
        switch (GameManager.Instance.CurrentGameState)
        {
            case GameState.Game:
                if (!isTimerInitialized) return; // 初期化が完了するまでUpdateTimerを呼ばない

                countTimer += Time.deltaTime; // 時間を計測する
                UpdateTimer(); // テキストに計測時間を反映
                break;
            case GameState.GameOver:
            case GameState.Clear:
                break;
        }
    }

    public void UpdateTimer()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(countTimer);
        timerText.text = string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
    }

    private IEnumerator SetActiveTrueTimer()
    {
        while (GameManager.Instance.CurrentGameState != GameState.Game) // ゲームが開始されるまで待機
        {
            yield return null;
        }

        OBJECT.SetActive(true); // オブジェクトをアクティブ化
        timerText = TimerText.GetComponent<TextMeshProUGUI>(); // テキストを取得

        if (timerText == null)
        {
            yield break;
        }

        isTimerInitialized = true;
    }
}
