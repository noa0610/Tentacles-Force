using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームのフラグを管理するシングルトン
/// </summary>
public enum GameState
{
    None,
    Game,
    GameOver,
    Clear
}

public class GameManager : MonoBehaviour
{

    public static GameManager Instance; // シングルトンインスタンスを保持


    public GameState currentGameState = GameState.None; // ゲーム状態
    public GameState CurrentGameState // ゲーム状態取得プロパティ
    {
        get { return currentGameState; }
        set
        {
            if (currentGameState != value)
            {
                Debug.Log($"GameState changed: {currentGameState} -> {value}");
                currentGameState = value;
                OnGameStateChanged?.Invoke(currentGameState);
            }
        }
    }
    public event Action<GameState> OnGameStateChanged; // ゲーム状態変更イベント


    void Awake()
    {
        // シングルトンインスタンスの設定
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンをまたいでも破棄されないようにする
        }
        else
        {
            Destroy(gameObject); // 既にインスタンスが存在する場合は、重複しないように破棄
        }
    }
}
