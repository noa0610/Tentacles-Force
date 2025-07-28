using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

/* 索敵を行いターゲットを追尾する敵の移動スクリプト
 *
 */
public class EnemyMove : MonoBehaviour
{
    // インスペクターから設定する変数
    [Header("移動能力設定")]
    [SerializeField] private float IdleMoveRange = 2f; // 往復移動の範囲
    [SerializeField] private float IdleSpeed = 5f; // 往復移動の速度
    [SerializeField] private float ContactHorizontalSpeed = 4f; // 横の最高移動速度
    [SerializeField] private float ContactVerticalSpeed = 2.5f; // 縦の最高移動速度
    [SerializeField] private float Acceleration = 10f; // 加速度
    [SerializeField] private float StraightAttackSpeed = 2f; // 直線攻撃の移動速度


    [Header("索敵能力設定")]
    [SerializeField] private float SearchRange = 7f;  // 索敵範囲
    [SerializeField] private float ContactRange = 10f;  // 接敵範囲
    [SerializeField] private LayerMask TargetLayerMask; // ターゲットのレイヤー


    [SerializeField] private float DamegeStopTime = 0.75f; // ダメージを受けた際の行動停止時間
    [SerializeField] private float AttackChangeTime = 0.5f; // 攻撃に移行するまでの時間
    [SerializeField] private float AttackTime = 1f; // 攻撃の持続時間

    [Header("最初の向き設定")]
    [SerializeField] private bool StartDirectionRight = true; // 初期の向き（右方向）

    /** 追加機能  */
    // 撃破されたことを通知するSubject
    private readonly Subject<Unit> _onDefeated = new Subject<Unit>();
    // イベントを外部に公開する仕組み
    public IObservable<Unit> OnDefeated => _onDefeated;
    public int scoreValue = 100;
    /**          */


    // 内部処理する変数
    private enum EnemyState
    {
        Idle,
        Contact,
        StraightAttack,
        Damage,
        Dead
    }
    private EnemyState currentState = EnemyState.Idle;

    private GameObject target; // 追跡するターゲット
    private Vector3 idleCenterPos; // 往復移動の中心位置
    private float ExitTimer = 0f; // ステート遷移のタイマー
    private Rigidbody2D _rigidbody2D;
    private Collider2D _collider2D;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private UnitBase _unitBase;
    private Attack _attack;
    private int _currentHealth; // ユニットの現在のHP

    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _unitBase = GetComponent<UnitBase>();
        _attack = GetComponentInChildren<Attack>();

        SetDirection(StartDirectionRight ? Vector3.right : Vector3.left); // 初期の向きを設定
        StateTransition(EnemyState.Idle); // 初期状態を待機状態に設定
    }

    void Start()
    {
        _currentHealth = _unitBase.UnitStatus.maxHealth; // 初期HPを設定
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                IdleState();
                break;
            case EnemyState.Contact:
                ContactState();
                break;
            case EnemyState.StraightAttack:
                StraightAttackState();
                break;
            case EnemyState.Damage:
                DamageState();
                break;
            case EnemyState.Dead:
                DeadState();
                break;
        }
    }

    /// <summary>
    /// 索敵を行いプレイヤーを探す待機状態
    /// </summary>
    private void IdleState()
    {
        // 索敵を行う
        TargetSearch(SearchRange);

        // ターゲットを発見したら
        if (target != null)
        {
            StateTransition(EnemyState.Contact);
        }

        // 往復移動
        IdleMove();

        if (_unitBase.CurrentHealth < _currentHealth)
        {
            // ダメージを受けた場合、ダメージ状態に移行
            StateTransition(EnemyState.Damage);
        }
    }

    /// <summary>
    /// 一定範囲にいるプレイヤーを追尾する追跡状態
    /// </summary>
    private void ContactState()
    {
        // 時間を計測
        ExitTimer += Time.deltaTime;

        // 索敵を行う
        TargetSearch(ContactRange);

        // ターゲットを発見し、攻撃移行時間まで経過したら
        if (target != null)
        {
            Rotation(target); // ターゲットの方向を向く
            ContactMove(target); // ターゲットを追跡する

            if (AttackChangeTime <= ExitTimer)
            {
                StateTransition(EnemyState.StraightAttack);
            }
        }

        // ターゲットを見失えば
        if (target == null)
        {
            // 待機状態に移行
            StateTransition(EnemyState.Idle);
        }

        if (_unitBase.CurrentHealth < _currentHealth)
        {
            // ダメージを受けた場合、ダメージ状態に移行
            StateTransition(EnemyState.Damage);
        }
    }

    private void StraightAttackState()
    {
        // 時間を計測
        ExitTimer += Time.deltaTime;

        // 回転方向に直線移動
        MoveStraightByRotation();

        if (ExitTimer >= AttackTime)
        {
            StateTransition(EnemyState.Contact);
        }

        if (_unitBase.CurrentHealth < _currentHealth)
        {
            // ダメージを受けた場合、ダメージ状態に移行
            StateTransition(EnemyState.Damage);
        }
    }

    /// <summary>
    /// ダメージを受けて一定時間行動を停止する状態
    /// </summary>
    private void DamageState()
    {
        // 時間を計測
        ExitTimer += Time.deltaTime;

        if (_unitBase.IsDead)
        {
            StateTransition(EnemyState.Dead);
        }

        // 停止時間まで経過したら
        if (ExitTimer >= DamegeStopTime)
        {
            StateTransition(EnemyState.Idle);
        }
    }

    /// <summary>
    /// 一定時間経過後に自オブジェクトを削除する死亡状態
    /// </summary>
    private void DeadState()
    {
        if (_spriteRenderer.flipY == false)
        {
            _spriteRenderer.flipY = true; // 死亡時にスプライトを反転
        }
    }

    // ステートの切り替え処理
    private void StateTransition(EnemyState newState)
    {
        if (currentState == EnemyState.Idle)
        {
            _animator.SetBool("Idle", false);
        }
        else if (currentState == EnemyState.Contact)
        {
            _rigidbody2D.velocity = Vector2.zero;
            ResetRotation(); // 回転をリセット
            _animator.SetBool("Contact", false);
        }
        else if (currentState == EnemyState.StraightAttack)
        {
            _rigidbody2D.velocity = Vector2.zero;
            ResetRotation(); // 回転をリセット
            _animator.SetBool("Attack", false);
            _attack.HitOff(); // 攻撃を無効にする
        }
        else if (currentState == EnemyState.Damage)
        {
            _animator.SetBool("Damage", false);
        }
        else if (currentState == EnemyState.Dead)
        {
            // 既に死亡状態なので何もしない
            return;
        }

        if (newState == EnemyState.Idle)
        {
            SetIdleCenter(transform.position); // 往復移動の中心位置を設定
            _animator.SetBool("Idle", true);
        }
        else if (newState == EnemyState.Contact)
        {
            SoundManager.Instance.PlaySE("決定ボタンを押す48_EnemyContact");
            ExitTimer = 0f;
            _animator.SetBool("Contact", true);
        }
        else if (newState == EnemyState.StraightAttack)
        {
            ExitTimer = 0f;
            Rotation(target); // ターゲットの方向を向く
            _animator.SetBool("Attack", true);
            _attack.IDListClear(); // ヒットIDリストをクリア
            _attack.HitOn(); // 攻撃を有効にする
        }
        else if (newState == EnemyState.Damage)
        {
            ExitTimer = 0f;
            _animator.SetBool("Damage", true);
        }
        else if (newState == EnemyState.Dead)
        {
            /** 追加機能 */
            _onDefeated.OnNext(Unit.Default); // 撃破を通知
            MessageBroker.Default.Publish(new EnemyDefeatedMessage(scoreValue, transform.position));
            _onDefeated.OnCompleted(); // 購読終了
            /**         */

            _rigidbody2D.velocity = Vector2.zero; // 速度をゼロにする
            _collider2D.enabled = false; // コライダーをオフ
            _animator.SetTrigger("Dead"); // 死亡アニメーションをトリガーする
            Destroy(gameObject, 2f); // 2秒後にオブジェクトを削除
        }

        Debug.Log($"State Transition: {currentState} -> {newState}");
        currentState = newState;
    }

    public void SetIdleCenter(Vector3 center)
    {
        idleCenterPos = center;
        // 現在位置が中心より右なら左向き、左なら右向きで開始
        SetDirection((transform.position.x >= idleCenterPos.x) ? Vector3.left : Vector3.right);
    }

    private void IdleMove()
    {
        float leftLimit = idleCenterPos.x - IdleMoveRange;
        float rightLimit = idleCenterPos.x + IdleMoveRange;

        // 端に到達したら進行方向を反転
        if (transform.position.x <= leftLimit)
            // 向きを切り替え
            SetDirection(new Vector3(1f, 0f, 0f));
        else if (transform.position.x >= rightLimit)
            // 向きを切り替え
            SetDirection(new Vector3(-1f, 0f, 0f));

        // 移動方向ベクトル
        Vector2 moveDir = new Vector2(GetDirection().x, 0f);

        // Rigidbody2Dで移動
        Vector2 velocity = moveDir * IdleSpeed;
        _rigidbody2D.velocity = new Vector2(velocity.x, _rigidbody2D.velocity.y);
    }

    // ターゲットを追跡する
    private void ContactMove(GameObject target)
    {
        Vector2 targetVelocity = new Vector2(
        GetHorizontalDirection(target).x * ContactHorizontalSpeed,
        GetVerticalDirection(target).y * ContactVerticalSpeed);

        Vector2 velocityDiff = targetVelocity - _rigidbody2D.velocity;
        Vector2 force = velocityDiff * Acceleration;

        _rigidbody2D.AddForce(force);

        // Optional: 速度が最大を超えないように制限（強めのブレーキ）
        _rigidbody2D.velocity = Vector2.ClampMagnitude(_rigidbody2D.velocity, ContactHorizontalSpeed);
    }

    // 円形の範囲上のターゲットを取得する
    private void TargetSearch(float searchRange)
    {
        Collider2D collider2D = Physics2D.OverlapCircle(this.transform.position, searchRange, TargetLayerMask);

        if (collider2D != null)
        {
            target = collider2D.gameObject;
        }
        else
        {
            target = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 索敵範囲を可視化
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, SearchRange);

        // 追跡範囲を可視化
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, ContactRange);
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

        // 自オブジェクトとターゲットのy座標を比較
        return target.transform.position.y >= transform.position.y ? Vector2.up : Vector2.down;
    }


    /// <summary>
    /// ターゲットの方向を向くように回転する処理
    /// </summary>
    /// <param name="target"></param>
    private void Rotation(GameObject target)
    {
        // 自オブジェクトの位置を取得
        Vector3 thisPos = this.transform.position;

        // ターゲットの位置を取得
        Vector3 targetPos = target.transform.position;

        // 自オブジェクトとターゲットのベクトルを計算
        Vector3 direction = (thisPos - targetPos).normalized;

        // ターゲットの位置が自オブジェクトより左にある場合
        if (thisPos.x < targetPos.x)
        {
            SetDirection(direction); // 向きをターゲット方向に設定
        }
        else
        {
            SetDirection(-direction); // 向きをターゲット方向に設定（反転）
        }

        // 回転を計算
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 自オブジェクトの回転を設定
        this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle)); // 90度回転させて右方向を基準にする
    }

    /// <summary>
    /// 自オブジェクトの回転を初期状態に戻す
    /// </summary>
    private void ResetRotation()
    {
        this.transform.rotation = Quaternion.identity;
    }

    /// <summary>
    /// 自オブジェクトの向きを指定したベクトルに合わせて反転させる
    /// </summary>
    public void SetDirection(Vector3 moveVec)
    {
        if (moveVec.x != 0f)
        {
            Vector3 newLocalScale = transform.localScale;
            newLocalScale.x = Mathf.Abs(newLocalScale.x) * (moveVec.x < 0 ? -1f : 1f);
            transform.localScale = newLocalScale;
        }
    }

    /// <summary>
    /// -1で左、1で右
    /// </summary>
    /// <returns></returns>
    public Vector3 GetDirection()
    {
        return transform.localScale.x < 0 ? Vector3.left : Vector3.right;
    }


    /// <summary>
    /// 現在の回転方向（Z軸）に向かって直線移動する
    /// </summary>
    private void MoveStraightByRotation()
    {
        // 現在のZ回転角度を取得
        float angle = transform.eulerAngles.z + 180f;
        // 角度をラジアンに変換
        float rad = angle * Mathf.Deg2Rad;
        // 回転方向の単位ベクトルを計算（右方向基準）
        Vector2 direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;

        // 直線攻撃速度で移動
        _rigidbody2D.velocity = direction * StraightAttackSpeed;
    }
}

// イベント用のメッセージクラス
public class EnemyDefeatedMessage
{
    public int ScoreValue;
    public Vector3 Position;

    public EnemyDefeatedMessage(int score, Vector3 position)
    {
        ScoreValue = score;
        Position = position;
    }
}