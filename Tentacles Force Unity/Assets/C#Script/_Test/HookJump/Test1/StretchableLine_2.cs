using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 衝突判定をプレイヤーに渡すフックのクラス
/// </summary>
public class StretchableLine_2 : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    [SerializeField] private HookJump _hookJump;
    private Transform startPoint;                       // フックの基点（プレイヤー）
    [SerializeField] private LineRenderer lineRenderer; // フックのロープの描画
    [SerializeField] private LayerMask hitMask;         // 命中判定用レイヤー
    private Rigidbody2D rb;                            // フックのRigidbody2D

    // 内部処理する変数
    private bool isHooked = false;     // フックが地形に命中したか
    private Vector3 hookHitPoint;      // フックが命中した位置
    private bool isMoving = false;     // フックが移動中かどうか
    private Vector3 targetPosition;     // 目標位置

    private void Awake()
    {
        // Rigidbody2Dの設定
        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.drag = 1f;  // 空気抵抗
        rb.angularDrag = 1f;  // 回転抵抗
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;  // 連続衝突検出
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;  // 補間

        // 呼び出されるまでオブジェクトを非アクティブ化
        gameObject.SetActive(false);
    }

    private void Start()
    {
        if (Player == null)
        {
            // プレイヤーのオブジェクトを探す
            Player = GameObject.FindWithTag("Player");
        }

        // プレイヤーのスクリプトを取得
        _hookJump = Player.GetComponent<HookJump>();

        // 現在のプレイヤーの位置とフックの位置で初期化
        startPoint = Player.gameObject.transform;
    }

    private void Update()
    {
        // プレイヤーの位置とフックの位置を更新し続ける
        startPoint = Player.gameObject.transform;

        // ロープの描画を更新
        UpdateLineRenderer(startPoint.position, this.transform.position);

        // ロープの当たり判定をRayで作る
        GetRayPoint();

        // フックの移動処理
        if (isMoving)
        {
            MoveHook();
        }
    }

    /// <summary>
    /// フックを目標位置に向かって移動させる
    /// </summary>
    public void MoveTo(Vector3 target)
    {
        targetPosition = target;
        isMoving = true;
    }

    /// <summary>
    /// フックの移動を停止
    /// </summary>
    public void StopMoving()
    {
        isMoving = false;
        rb.velocity = Vector2.zero;
    }

    /// <summary>
    /// フックの移動処理
    /// </summary>
    private void MoveHook()
    {
        Vector2 direction = (targetPosition - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, targetPosition);

        // 目標位置に近づいたら停止
        if (distance < 0.1f)
        {
            StopMoving();
            return;
        }

        // 目標位置に向かって力を加える
        float force = distance * 10f;  // 距離に応じた力の調整
        rb.AddForce(direction * force, ForceMode2D.Force);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 地面や壁に命中したとき、フックが巻き戻っていないとき、フックが発射されているとき、
        if (other.gameObject.CompareTag("Ground") && _hookJump.IsHookRewind == false && _hookJump.IsHookFired)
        {
            // フックの移動を停止
            StopMoving();

            // フックが命中した地点を記録
            hookHitPoint = other.ClosestPoint(this.transform.position);

            // フックが命中した判定
            isHooked = true;

            // 命中した地点をプレイヤーに渡す
            _hookJump.HookTargetPosition = hookHitPoint;

            // 命中した判定をプレイヤーに渡す
            _hookJump.IsHookHit = isHooked;

            Debug.Log($"HookHit: {hookHitPoint}");
        }
    }

    /// <summary>
    /// レイを飛ばしロープの当たり判定を作る
    /// </summary>
    private void GetRayPoint()
    {
        // 自分とプレイヤーの位置の2点で繋いでレイを飛ばす
        RaycastHit2D hit = Physics2D.Linecast(Player.transform.position, this.transform.position, hitMask);

        // 命中した場合
        if (hit.collider != null && _hookJump.IsHookRewind == false && _hookJump.IsHookFired)
        {
            // フックの移動を停止
            StopMoving();

            // 命中地点を小さな十字で可視化
            float crossSize = 0.2f;  // 十字のサイズ
            Vector2 hitPoint = hit.point;

            Debug.DrawLine(hitPoint + Vector2.left * crossSize, hitPoint + Vector2.right * crossSize, Color.cyan, 1f);
            Debug.DrawLine(hitPoint + Vector2.up * crossSize, hitPoint + Vector2.down * crossSize, Color.cyan, 1f);

            Debug.DrawLine(Player.transform.position, hit.point, Color.red, 1f);  // ヒット地点まで赤色
            Debug.DrawLine(hit.point, this.transform.position, Color.green, 1f);  // ヒット後の部分は緑色

            // 命中した地点を記録
            hookHitPoint = hit.point;

            // フックが命中した判定
            isHooked = true;

            // 命中した地点をプレイヤーに渡す
            _hookJump.HookTargetPosition = hookHitPoint;

            // 命中した判定をプレイヤーに渡す
            _hookJump.IsHookHit = isHooked;
        }
        // 命中しない場合
        else
        {
            Debug.DrawLine(this.transform.position, Player.transform.position, Color.blue, 1f);  // ヒットしない場合は青色
        }
    }

    /// <summary>
    /// フックの地点までロープを描画する
    /// </summary>
    private void UpdateLineRenderer(Vector3 startingPoint, Vector3 endingPoint)
    {
        // ロープの始点
        lineRenderer.SetPosition(0, startingPoint);

        // ロープの終点
        lineRenderer.SetPosition(1, endingPoint);
    }
}
