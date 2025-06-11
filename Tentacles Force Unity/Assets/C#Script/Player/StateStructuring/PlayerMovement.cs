using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの移動能力を纏めたクラス
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    
    private readonly float GroundAccel = 20f;      // 地上の加速力
    private readonly float MaxGroundSpeed = 14f;   // 地上の最大移動速度
    private readonly float GroundBrakeForce = 25f; // 地上の移動停止力
    private readonly float minVelocity = 0.1f;     // ブレーキをかける最小速度（この値になったら完全停止）

    private readonly float AirAccel = 8f;          // 空中の加速力
    private readonly float MaxAirSpeed = 6f;       // 空中の最大移動速度
    private readonly float TurnBrakeForce = 4f;    // 方向転換時のブレーキ力

    private readonly float RayLange = 0.4f;        // Rayの長さ（地面判定用）
    private readonly float JumpForce = 15f;        // ジャンプ力

    private Rigidbody2D _rigidbody2D;
    private Vector3 _facingDirection = Vector3.right; // 現在向いている方向
    private Vector2 _horizontalVelocity; // 水平方向の速度
    private Vector2 _verticalVelocity;   // 垂直方向の速度

    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }


    /// <summary>
    /// 水平方向の速度を計算する（移動ベクトル、加速度、最大速度を指定）
    /// </summary>
    /// <param name="moveVec"></param>
    /// <param name="accel"></param>
    /// <param name="maxSpeed"></param>
    private void CalcHorizontalVelocity(Vector3 moveVec, float accel, float maxSpeed)
    {
        _horizontalVelocity = Vector3.MoveTowards(
                                _horizontalVelocity,
                                moveVec * maxSpeed,
                                accel * Time.deltaTime);
    }

    /// <summary>
    /// 水平方向の速度を減速させる（加速度を指定）
    /// </summary>
    /// <param name="accel"></param>
    private void CalcHorizontalVelocity(float accel)
    {
        _horizontalVelocity = Vector3.MoveTowards(
                                _horizontalVelocity,
                                Vector3.zero,
                                accel * Time.deltaTime);
    }

    /// <summary>
    /// プレイヤーの向きを変更する
    /// </summary>
    /// <param name="moveVec"></param>
    public void Direction(Vector3 moveVec)
    {
        if (moveVec.x != 0f)
        {
            bool isLeft = moveVec.x < 0;
            Vector3 newLocalScale = transform.localScale;
            if (isLeft && newLocalScale.x > 0 || !isLeft && newLocalScale.x < 0)
            {
                // 向きが変わる場合のみ処理
                newLocalScale.x *= -1f; // スケールを反転させて向きを変える
                transform.localScale = newLocalScale;
            }
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
    /// 地上移動を行う
    /// </summary>
    /// <param name="moveVec"></param>
    public void GroundMove(Vector3 moveVec)
    {
        CalcHorizontalVelocity(moveVec, GroundAccel, MaxGroundSpeed);
    }

    /// <summary>
    /// 地上移動を行う（加速倍率指定可能）
    /// </summary>
    /// <param name="moveVec"></param>
    /// <param name="addAccelMultiple"></param>
    public void GroundMove(Vector3 moveVec, float addAccelMultiple)
    {
        CalcHorizontalVelocity(moveVec, GroundAccel * addAccelMultiple, MaxGroundSpeed * addAccelMultiple);
    }

    /// <summary>
    /// スライド移動（攻撃モーション時の移動など）
    /// </summary>
    public void GroundMoveBrake()
    {
        CalcHorizontalVelocity(GroundBrakeForce);
    }

    /// <summary>
    /// 空中移動を行う
    /// </summary>
    /// <param name="moveVec"></param>
    public void AirMove(Vector3 moveVec)
    {
        CalcHorizontalVelocity(moveVec, AirAccel, MaxAirSpeed);
    }

    /// <summary>
    /// ジャンプ処理（Y軸方向の速度をセット）
    /// </summary>
    public void Jump()
    {
        _verticalVelocity.y = JumpForce;
    }

    public void ActionJump(Vector2 direction, float jumpForce)
    {
        _verticalVelocity = direction * jumpForce;
        _horizontalVelocity = direction * jumpForce;
    }
    
    public void SetVelocityZero()
    {
        _horizontalVelocity.y = 0f;
    }

    /// <summary>
    /// 空中にいるかを判定する
    /// </summary>
    /// <returns></returns>
    public bool AirJudge()
    {
        var collider = GetComponent<Collider2D>(); // プレイヤーのコライダー取得
        float rayStartHeight = collider.bounds.min.y; // コライダーの底＋少し浮かせる
        var rayStart = new Vector3(transform.position.x, rayStartHeight, transform.position.z);
        var ray = new Ray(rayStart, Vector3.down);
        var layerMask = LayerMask.GetMask("Ground");


        // プレイヤーの下方向にSphereCastを行い、地面との接触を判定
        if (!Physics2D.Raycast(rayStart, Vector2.down * 0.2f, RayLange, layerMask))
        {
            Debug.DrawRay(rayStart, Vector2.down * 0.2f, Color.green, RayLange);
            return true; // 地面に接触していない（空中）
        }
        Debug.DrawRay(rayStart, Vector2.down * 0.2f, Color.red, RayLange);
        return false; // 地面に接触している
    }

    /// <summary>
    /// 現在のRigidbodyの速度を取得して分解
    /// </summary>
    public void GetVelocity()
    {
        var velocity = _rigidbody2D.velocity;
        _horizontalVelocity = new Vector3(velocity.x, 0.0f, 0.0f);
        _verticalVelocity = new Vector3(0.0f, velocity.y, 0.0f);
    }

    /// <summary>
    /// 計算した速度をRigidbodyに適用
    /// </summary>
    public void SetVelocity()
    {
        _rigidbody2D.velocity = _horizontalVelocity + _verticalVelocity;
    }

    /// <summary>
    /// 速度をリセットする（Rigidbodyの速度を0にする）
    /// </summary>
    public void ResetVelocity()
    {
        _horizontalVelocity = Vector3.zero;
        _verticalVelocity = Vector3.zero;
    }
}
