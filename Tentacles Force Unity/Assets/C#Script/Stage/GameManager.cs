using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームのフラグを管理するシングルトン
/// </summary>
public class GameManager : MonoBehaviour
{

    public static GameManager Instance; // シングルトンインスタンスを保持

    // 内部処理する変数
    private bool isGame = false; // ゲーム中フラグ
    private bool isGOAL = false; // ゴールフラグ
    private bool isGameOver = false; // ゲームオーバーフラグ
    public event Action<bool> OnIsGameChanged;

    // プロパティ
    public bool IsGame // ゲーム中フラグ取得プロパティ
    {
        get { return isGame; }
        set
        {
            if (isGame != value)
            {
                Debug.Log($"IsGame changed: {isGame} -> {value}");
                isGame = value;
                OnIsGameChanged?.Invoke(isGame); // 状態が変化したら通知
            }
        }
    }
    public bool IsGOAL // ゴールフラグ取得プロパティ
    {
        get { return isGOAL; }
        set { isGOAL = value; }
    }
    public bool IsGameOver // ゲームオーバーフラグ取得プロパティ
    {
        get { return isGameOver; }
        set { isGameOver = value; }
    }

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
