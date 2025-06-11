using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;

/// <summary>
/// 衝突判定をプレイヤーに渡すフックのクラス
/// </summary>
public class StretchableLine_2 : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    [SerializeField] private HookJump _hookJump;
    private Transform startPoint;                       // フックの基点（プレイヤー）
    [SerializeField] private LineRenderer lineRenderer; // フックのロープの描画
    [SerializeField] private LayerMask HtiLayer;        // 命中判定用レイヤー（敵と地面のレイヤー選択）

    // 内部処理する変数
    private bool isHooked = false;     // フックが地形に命中したか
    private Vector3 hookHitPoint;      // フックが命中した位置

    private Transform PlayerPosition;  // プレイヤー位置
    private Transform HookPosition;    // フック位置
    private Vector3 lastHookPosition;  // 前フレームフック位置
    private PolygonCollider2D polygonCollider; // ポリゴンコライダー
    private GameObject colliderObject; // コライダー用オブジェクト

    private void Awake()
    {
        // 呼び出されるまでオブジェクトを非アクティブ化
        gameObject.SetActive(false);

        /* コライダーの初期設定 */
        // コライダー用オブジェクトを生成
        colliderObject = new GameObject("HookCollider");
        colliderObject.transform.SetParent(this.transform);
        // ポリゴンコライダーの初期設定
        polygonCollider = colliderObject.AddComponent<PolygonCollider2D>();
        // トリガー化
        polygonCollider.isTrigger = true;

        // 初期コライダーを設定
        InitializeCollider();

        // 初期位置を記録
        lastHookPosition = transform.position;
    }

    /// <summary>
    /// コライダーを初期化する
    /// </summary>
    private void InitializeCollider()
    {
        // デフォルトの頂点を設定（小さな三角形）
        Vector2[] points = new Vector2[] {
            new Vector2(-0.1f, -0.1f),
            new Vector2(0.1f, -0.1f),
            new Vector2(0, 0.1f)
        };

        // 頂点を設定
        polygonCollider.points = points;
    }

    private void OnEnable()
    {
        // オブジェクトがアクティブ化されたときにコライダーを初期化
        InitializeCollider();
    }

    private void OnDisable()
    {
        // オブジェクトが非アクティブ化されたときにコライダーをクリア
        if (polygonCollider != null)
        {
            polygonCollider.points = new Vector2[0];
        }
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

        HookPosition = this.transform;
        PlayerPosition = Player.transform;
    }

    private void Update()
    {
        // プレイヤーの位置とフックの位置を更新し続ける
        startPoint = Player.gameObject.transform;

        colliderObject.transform.position = this.transform.position;

        HookPosition = this.transform;
        PlayerPosition = Player.transform;

        // ロープの描画を更新
        UpdateLineRenderer(startPoint.position, this.transform.position);

        // ロープの当たり判定をRayで作る
        GetRayPoint();

        // メッシュを作りロープの軌跡の当たり判定を補間
        // CreateMash();

        // 前フレームを更新
        PositionUpdate();
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
        RaycastHit2D hit = Physics2D.Linecast(Player.transform.position, this.transform.position, HtiLayer);

        // 命中した場合
        if (hit.collider != null && _hookJump.IsHookRewind == false && _hookJump.IsHookFired)
        {
            // オブジェクトのレイヤー取得
            int layer = hit.collider.gameObject.layer;

            if (layer == LayerMask.NameToLayer("Ground"))
            {
                // 命中した地点を記録
                hookHitPoint = hit.point;

                // フックが命中した判定
                isHooked = true;

                // 命中した地点をプレイヤーに渡す
                _hookJump.HookTargetPosition = hookHitPoint;

                // 命中した判定をプレイヤーに渡す
                _hookJump.IsHookHit = isHooked;
            }
            else if (layer == LayerMask.NameToLayer("Ground"))
            {
                // 命中した判定をプレイヤーに渡す
                _hookJump.IsHookHit = isHooked;
            }

            // 命中地点を小さな十字で可視化
            float crossSize = 0.2f;  // 十字のサイズ
            Vector2 hitPoint = hit.point;

            Debug.DrawLine(hitPoint + Vector2.left * crossSize, hitPoint + Vector2.right * crossSize, Color.cyan, 1f);
            Debug.DrawLine(hitPoint + Vector2.up * crossSize, hitPoint + Vector2.down * crossSize, Color.cyan, 1f);

            Debug.DrawLine(Player.transform.position, hit.point, Color.red, 1f);  // ヒット地点まで赤色
            Debug.DrawLine(hit.point, this.transform.position, Color.green, 1f);  // ヒット後の部分は緑色


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

    /// <summary>
    /// コライダーを生成し、判定を取得する
    /// </summary>
    // private void CreateMash()
    // {
    //     // 前フレーム位置の更新が無ければ処理しない
    //     if (lastHookPosition == HookPosition.position)
    //         return;

    //     // ローカル座標に変換した頂点を設定
    //     Vector2[] points = new Vector2[] {
    //         transform.InverseTransformPoint(lastHookPosition),    // 前フレームのフック位置
    //         transform.InverseTransformPoint(PlayerPosition.position), // プレイヤー位置
    //         transform.InverseTransformPoint(HookPosition.position)    // 現在のフック位置
    //     };

    //     // 頂点が重複していないかチェック
    //     if (points[0] == points[1] || points[1] == points[2] || points[0] == points[2])
    //     {
    //         Debug.LogWarning("Collider points are not distinct. Skipping collider creation.");
    //         return;
    //     }

    //     // 頂点間の距離が最小値以上あるかチェック
    //     float minDistance = 0.01f; // 最小距離の閾値
    //     if (Vector2.Distance(points[0], points[1]) < minDistance ||
    //         Vector2.Distance(points[1], points[2]) < minDistance ||
    //         Vector2.Distance(points[0], points[2]) < minDistance)
    //     {
    //         Debug.LogWarning("Collider points are too close. Skipping collider creation.");
    //         return;
    //     }

    //     // 頂点を設定
    //     polygonCollider.points = points;

    //     // デバッグ用に頂点の座標をログに出力
    //     Debug.Log($"Collider Points: p1={points[0]}, p2={points[1]}, p3={points[2]}");
    // }

    /// <summary>
    /// 位置を更新
    /// </summary>
    private void PositionUpdate()
    {
        // 前フレームの位置を更新する前に、現在の位置が有効かチェック
        if (HookPosition != null && HookPosition.position != Vector3.zero)
        {
            lastHookPosition = HookPosition.position;
        }
    }
}
