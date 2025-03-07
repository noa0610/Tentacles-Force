using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StretchableLine_2 : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    [SerializeField] private HookJump _hookJump;
    private Transform startPoint;                       // フックの基点（プレイヤー）
    [SerializeField] private LineRenderer lineRenderer; // フックのロープの描画
    [SerializeField] private LayerMask hitMask;         // 命中判定用レイヤー

    // 内部処理する変数
    private bool isHooked = false;     // フックが地形に命中したか
    private Vector3 hookHitPoint;      // フックが命中した位置

    private void Awake()
    {
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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"_hookJump.IsHookRewind : {_hookJump.IsHookRewind}");
        Debug.Log($"_hookJump.IsHookFired : {_hookJump.IsHookFired}");

        // 地面や壁に命中したとき、フックが巻き戻っていないとき、フックが発射されているとき、
        if (other.gameObject.CompareTag("Ground") && _hookJump.IsHookRewind == false && _hookJump.IsHookFired)
        {
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
        if (hit.collider != null &&  _hookJump.IsHookRewind == false && _hookJump.IsHookFired)
        {
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
