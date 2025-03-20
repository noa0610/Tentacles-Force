using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの移動能力を纏めたクラス
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    // TODO 空中での動作の処理を作る（もしくは既存の処理を地上、空中で処理が切り替わるように変更する）
    private readonly float GroundAccel = 10f;     // 地上の加速力
    private readonly float MaxGroundSpeed = 8f;   // 地上の最大移動速度
    private readonly float GroundBrakeForce = 2f; // 地上の移動停止力
    private readonly float minVelocity = 0.1f;    // ブレーキをかける最小速度（この値になったら完全停止）

    private readonly float AirAccel = 8f;        // 空中の加速力
    private readonly float MaxAirSpeed = 6f;      // 空中の最大移動速度
    private readonly float TurnBrakeForce = 4f;   // 方向転換時のブレーキ力

    private readonly float JumpForce = 15f;       // ジャンプ力

    private Rigidbody2D _rigidbody2D;
    private Vector2 _horizontalVelocity; // 水平方向の速度
    private Vector2 _verticalVelocity;   // 垂直方向の速度

    private Vector3 _currentMoveInput;   // 現在の入力方向を保持
    // プロパティで入力を管理
    public Vector3 CurrentMoveInput
    {
        get => _currentMoveInput;
        set
        {
            // 入力が変化した時の処理
            if (_currentMoveInput != value)
            {
                OnMoveInputChanged(value);
            }
            _currentMoveInput = value;
        }
    }
    private void OnMoveInputChanged(Vector3 newInput)
    {
        // ブレーキ中に入力が変化した場合の処理
        if (_brakeCoroutine != null)
        {
            StopCoroutine(_brakeCoroutine);
            _brakeCoroutine = null;
        }
    }

    private Coroutine _brakeCoroutine;   // 方向転換ブレーキのコルーチンを保持

    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }


    /// <summary>
    /// 地上での左右移動
    /// </summary>
    /// <param name="moveVec">移動する方向</param>
    public void GroundSideMove(Vector3 moveVec)
    {
        Vector2 velocity = _rigidbody2D.velocity;

        // 現在の速度が最大値を超えていなければ
        if (Mathf.Abs(velocity.x) < MaxGroundSpeed)
        {
            // 継続的に力を加えて移動
            _rigidbody2D.AddForce(new Vector2(moveVec.x * GroundAccel, 0), ForceMode2D.Force);
        }
    }

    /// <summary>
    /// 移動を徐々に停止させる
    /// </summary>
    public void GroundMoveStop()
    {
        // 既にストップ用のコルーチンが動いていれば処理しない
        if (_brakeCoroutine != null) return;

        // 停止処理を開始
        _brakeCoroutine = StartCoroutine(NaturalMoveStop());
    }
    private IEnumerator NaturalMoveStop()
    {
        // 停止処理開始時の移動方向を保存
        float initialDirection = Mathf.Sign(_rigidbody2D.velocity.x);

        // 速度が最小値以上の間、処理を続ける
        while (Mathf.Abs(_rigidbody2D.velocity.x) > minVelocity)
        {
            // 現在の速度を取得
            Vector2 velocity = _rigidbody2D.velocity;

            // 現在の移動方向をチェック
            float currentDirection = Mathf.Sign(velocity.x);

            // 移動方向が反転した場合は処理を終了
            if (currentDirection != initialDirection)
            {
                break;
            }

            // 入力があった場合は処理を終了
            if (_currentMoveInput.x != 0)
            {
                break;
            }

            // 現在の速度に応じた減速力を計算
            float brakeForce = Mathf.Abs(velocity.x) * GroundBrakeForce;

            // 速度が小さくなるほど減速力を弱める（より自然な停止に見えるよう調整）
            float speedRatio = Mathf.Abs(velocity.x) / MaxGroundSpeed;
            brakeForce *= Mathf.Lerp(0.5f, 1f, speedRatio);

            // 現在の進行方向と逆向きに力を加えて減速
            _rigidbody2D.AddForce(new Vector2(-currentDirection * brakeForce, 0), ForceMode2D.Force);

            yield return null;
        }

        // 完全に停止（微小な速度を0にする）
        _rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);
        _brakeCoroutine = null;
    }

    /// <summary>
    /// 振り向き
    /// </summary>
    /// <param name="turnVec">向く方向</param>
    public void Turnaround(Vector3 turnVec)
    {
        // 入力がない場合は処理を終了
        if (turnVec.x == 0) return;

        // Y軸の回転を変更（右向きが0度、左向きが180度）
        float rotationY = turnVec.x > 0 ? 0 : 180;
        transform.rotation = Quaternion.Euler(0, rotationY, 0);
    }

    /// <summary>
    /// 方向転換するとき、徐々に速度を落とすブレーキ処理を開始
    /// </summary>
    public void MoveBrake(Vector3 moveVec)
    {
        Vector2 velocity = _rigidbody2D.velocity;

        // 入力がない場合は処理を行わない
        if (moveVec.x == 0) return;

        // 移動方向とは逆方向への入力があり、かつ現在の速度が一定以上ならブレーキ処理開始
        if (Mathf.Sign(velocity.x) != Mathf.Sign(moveVec.x) && Mathf.Abs(velocity.x) > 0.1f)
        {
            // ブレーキ中であれば
            if (_brakeCoroutine != null)
            {
                // ブレーキ処理を止める
                StopCoroutine(_brakeCoroutine);
            }
            // ブレーキ処理を開始する
            _brakeCoroutine = StartCoroutine(TurnBrake());
        }
    }
    private IEnumerator TurnBrake()
    {
        // 方向転換前の移動方向を保存
        float initialDirection = Mathf.Sign(_rigidbody2D.velocity.x);

        // 方向転換時の入力方向(直前の移動方向の逆)を保存
        float initialInputDirection = Mathf.Sign(_currentMoveInput.x);

        // 速度がほぼ0になるまで処理
        while (Mathf.Abs(_rigidbody2D.velocity.x) > minVelocity)
        {
            // 現在の速度を取得
            Vector2 velocity = _rigidbody2D.velocity;

            // 現在の移動方向をチェック
            float currentDirection = Mathf.Sign(velocity.x);

            // 現在の入力方向をチェック
            float currentInputDirection = Mathf.Sign(_currentMoveInput.x);

            // 移動方向が反転したか、入力方向が変化した場合
            if (currentDirection != initialDirection ||
                currentInputDirection != initialInputDirection)
            {
                break;
            }

            // 速度の向きとは逆方向にブレーキ力を加える
            float brakeForce = Mathf.Abs(velocity.x) * TurnBrakeForce; // 速度に比例した減速力
            _rigidbody2D.AddForce(new Vector2(-currentDirection * brakeForce, 0), ForceMode2D.Force);

            yield return null;
        }

        // 完全に停止
        _rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);
        _brakeCoroutine = null;
    }


    /// <summary>
    /// ジャンプ処理
    /// </summary>
    public void GroundJump()
    {
        // 既存の上下速度をリセット
        _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0);

        // 瞬間的に力を加えてジャンプ
        _rigidbody2D.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
    }

    /// <summary>
    /// 特殊なアクションによるジャンプ処理
    /// </summary>
    /// <param name="direciton">単位ベクトル</param>
    /// <param name="jumpForce">ジャンプ力</param>
    public void ActionJump(Vector2 direciton, float jumpForce)
    {
        // 現在の速度をリセット
        _rigidbody2D.velocity = new Vector2(0, 0);

        // 単位ベクトルの方向にジャンプ
        _rigidbody2D.AddForce(direciton * jumpForce, ForceMode2D.Impulse);
    }

    /// <summary>
    /// 地面判定取得
    /// </summary>
    public bool CheckGround()
    {
        bool _isGround;

        float rayLength = 0.2f;

        var layerMask = LayerMask.GetMask(new string[] { "Ground" });

        // コライダーを取得
        Collider2D col = GetComponent<Collider2D>();

        // コライダーの最下の位置を取得
        Vector2 bottomPosition = new Vector2(col.bounds.center.x, col.bounds.min.y);

        // レイを下方向に発射して、地面に接触したか判定を行う
        _isGround = Physics2D.Raycast(bottomPosition, Vector2.down, rayLength, layerMask);

        // シーン上でレイを表示（緑で表示、判定取得で赤で表示）
        Debug.DrawRay(bottomPosition, Vector2.down * rayLength, _isGround ? Color.green : Color.red);

        return _isGround;
    }
}
