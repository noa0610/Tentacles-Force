using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    // インスペクターから設定する変数
    [SerializeField] private float HorizontalMoveForce = 3f;    // 横の移動力
    [SerializeField] private float MaxHorizontalMoveSpeed = 4f; // 横の最高移動速度
    [SerializeField] private float VerticalMoveForce = 2f;    // 縦の移動力
    [SerializeField] private float MaxVerticalMoveSpeed = 2.5f; // 縦の最高移動速度
    [SerializeField] private float Acceleration = 10f;
    [SerializeField] private float Damping = 0.9f;

    [SerializeField] private float SearchRange = 7f;  // 索敵範囲
    [SerializeField] private float ChaseRange = 10f;  // 追跡範囲
    [SerializeField] private LayerMask TargetLayerMask; // ターゲットのレイヤー
    [SerializeField] private float searchInterval = 0.2f; // 索敵を行う間隔


    // 内部処理する変数
    private enum EnemyState
    {
        Idle,
        Chase
    }
    private EnemyState currentState = EnemyState.Idle;
    private GameObject target; // 追跡するターゲット
    private float searchTimer = 0f;
    private Rigidbody2D _rigidbody2D;
    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                IdleState();
                break;
            case EnemyState.Chase:
                // 追跡移動を開始
                ChaseState();
                break;
        }
    }

    // 待機状態
    private void IdleState()
    {
        // 一定間隔で索敵を行う
        searchTimer += Time.deltaTime;
        if (searchTimer >= searchInterval)
        {
            searchTimer = 0f;
            TargetSearch(SearchRange);
        }

        // ターゲットを発見したら
        if (target != null)
        {
            // 追跡状態に移行
            currentState = EnemyState.Chase;
        }
        else
        {
            _rigidbody2D.velocity = Vector2.zero;
        }
    }

    // 追跡状態
    private void ChaseState()
    {
        // 一定間隔で索敵を行う
        searchTimer += Time.deltaTime;
        if (searchTimer >= searchInterval)
        {
            searchTimer = 0f;
            TargetSearch(ChaseRange);
        }

        // ターゲットを見失えば
        if (target == null)
        {
            // 待機状態に移行
            currentState = EnemyState.Idle;
            return;
        }

        // ターゲットを追跡する
        ChaseMove(target);
    }

    private void StateTransition(EnemyState newState)
    {
        if (currentState == EnemyState.Chase)
        {

        }
        else if (currentState == EnemyState.Idle)
        { 

        }

        if (newState == EnemyState.Chase)
        {

        }
        else if (newState == EnemyState.Idle)
        {

        }
        
        // 状態遷移時の処理（必要に応じて）
            currentState = newState;
    }

    // ターゲットを追跡する
    private void ChaseMove(GameObject target)
    {
        Vector2 targetVelocity = new Vector2(
        GetHorizontalDirection(target).x * MaxHorizontalMoveSpeed,
        GetVerticalDirection(target).y * MaxVerticalMoveSpeed);

        Vector2 velocityDiff = targetVelocity - _rigidbody2D.velocity;
        Vector2 force = velocityDiff * Acceleration;

        _rigidbody2D.AddForce(force);

        // Optional: 速度が最大を超えないように制限（強めのブレーキ）
        _rigidbody2D.velocity = Vector2.ClampMagnitude(_rigidbody2D.velocity, MaxHorizontalMoveSpeed);
    }

    // 円形の範囲上のターゲットを取得する
    private void TargetSearch(float searchRange)
    {
        Collider2D collider2D = Physics2D.OverlapCircle(this.transform.position, searchRange, TargetLayerMask);

        // コライダー取得ができた場合
        if (collider2D != null)
        {
            // ターゲットオブジェクト取得
            target = collider2D.gameObject;
        }
        else // コライダー範囲にいない場合
        {
            // ターゲットオブジェクト削除
            target = null;
        }
    }

    private Vector2 GetHorizontalDirection(GameObject target)
    {
        if (target == null) return Vector2.zero;

        // 自オブジェクトとターゲットのX座標を比較
        return target.transform.position.x >= transform.position.x ? Vector2.right : Vector2.left;
    }

    private Vector2 GetVerticalDirection(GameObject target)
    {
        if (target == null) return Vector2.zero;

        // 自オブジェクトとターゲットのX座標を比較
        return target.transform.position.y >= transform.position.y ? Vector2.up : Vector2.down;
    }
}
