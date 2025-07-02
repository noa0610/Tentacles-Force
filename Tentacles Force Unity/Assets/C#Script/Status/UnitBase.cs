using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    public UnitStatusDate unitStatus; // ユニットのステータスデータ
    public UnitStatusDate UnitStatus
    {
        get { return unitStatus; }
        set { unitStatus = value; }
    }
    public float InvincibleTime = 1f; // 無敵時間（秒）

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private string _currentHealthDebug; // デバッグ用の現在HP表示
#endif


    private int _currentHealth; // ユニットの現在のHP
    public int CurrentHealth
    {
        get { return _currentHealth; }
    }
    private bool _isDead = false; // ユニットが死亡しているかどうか
    public bool IsDead
    {
        get { return _isDead; }
    }
    private bool _isInvincible = false; // ユニットが無敵状態かどうか
    public bool IsInvincible
    {
        get { return _isInvincible; }
    }
    private Coroutine _invincibleCoroutine; // 無敵状態のコルーチン


    private void Start()
    {
        if (unitStatus != null)
        {
            _currentHealth = unitStatus.maxHealth; // 初期HPを設定

#if UNITY_EDITOR
            _currentHealthDebug = "Current Health: " + _currentHealth; // デバッグ用の現在HP表示
#endif

        }
        else
        {
            Debug.LogError("UnitStatus is not assigned for " + gameObject.name);
        }
    }

    public void TakeDamage(int damage)
    {
        if (_isDead || _isInvincible) return;

        int damageTaken = damage - unitStatus.defensePower; // 防御力を考慮したダメージ計算
        if (damageTaken < 0) damageTaken = 0; // ダメージが0未満にならないようにする
        _currentHealth -= damageTaken; // ダメージを受ける

#if UNITY_EDITOR
        _currentHealthDebug = "Current Health: " + _currentHealth; // デバッグ用の現在HP表示
#endif

        if (_currentHealth <= 0)
        {
            Daed(); // HPが0以下になったら死亡処理
        }

        if (_invincibleCoroutine != null)
        {
            StopCoroutine(_invincibleCoroutine);
        }

        _invincibleCoroutine = StartCoroutine(InvincibilityCoroutine()); // 無敵状態のコルーチンを開始
    }

    private IEnumerator InvincibilityCoroutine()
    {
        _isInvincible = true;
        yield return new WaitForSeconds(InvincibleTime);
        _isInvincible = false;
        _invincibleCoroutine = null; // コルーチンを終了
    }

    public void InvincibilityOn()
    {
        // 既存の無敵コルーチンがあれば停止
        if (_invincibleCoroutine != null)
        {
            StopCoroutine(_invincibleCoroutine);
            _invincibleCoroutine = null;
        }
        _isInvincible = true;
    }

    public void InvincibilityOff()
    {
        // 既存の無敵コルーチンがあれば停止
        if (_invincibleCoroutine != null)
        {
            StopCoroutine(_invincibleCoroutine);
            _invincibleCoroutine = null;
        }
        _isInvincible = false;
    }

    private void Daed()
    {
        // 死亡処理をここに追加
        _isDead = true;
    }
}
