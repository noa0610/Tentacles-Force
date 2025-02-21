using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StretchableLine_2 : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    [SerializeField] private HookJump _hookJump;
    private Transform startPoint;                       // フックの基点（プレイヤー）
    private Transform endPoint;
    [SerializeField] private LineRenderer lineRenderer; // フックのロープの描画
    [SerializeField] private float extendSpeed = 10f;
    [SerializeField] private float retractSpeed = 5f;   // 縮む速度
    [SerializeField] private LayerMask hitMask;         // 命中判定用レイヤー

    // 内部処理する変数
    private Vector2 targetPoint;       // フックの現在のターゲット地点
    private bool isExtending = false;  // フックを伸ばしているか
    private bool isRetracting = false; // フックを縮めているか
    private bool isHooked = false;     // フックが地形に命中したか
    private Vector3 hookHitPoint;      // フックが命中した位置

    private void Awake()
    {
        // 呼び出されるまでオブジェクトを非アクティブ化
        gameObject.SetActive(false);
    }

    private void Start()
    {
        if(Player == null)
        {
            // プレイヤーのオブジェクトを探す
            Player = GameObject.FindWithTag("Player");
        }

        // プレイヤーのスクリプトを取得
        _hookJump = Player.GetComponent<HookJump>();

        // 現在のプレイヤーの位置とフックの位置で初期化
        startPoint = Player.gameObject.transform;
        endPoint = this.transform;
    }

    private void Update()
    {
        // プレイヤーの位置とフックの位置を更新し続ける
        startPoint = Player.gameObject.transform;

        Debug.Log($"{endPoint.position}");
        
        
        // フックの伸び縮み
        // if (isExtending)
        // {
        //     // フックを伸ばす
        //     ExtendHook();
        // }
        // else if (isRetracting)
        // {
        //     // フックを縮める
        //     RetractHook();
        // }

        // ロープの描画を更新
        UpdateLineRenderer(startPoint.position, this.transform.position);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"_hookJump.IsHookRewind : {_hookJump.IsHookRewind}");
        Debug.Log($"_hookJump.IsHookFired : {_hookJump.IsHookFired}");

        // 地面や壁に命中したとき、フックが巻き戻っていないとき、フックが発射されているとき、
        if(other.gameObject.CompareTag("Ground") && _hookJump.IsHookRewind == false && _hookJump.IsHookFired)
        {
            // フックが命中した地点を記録
            hookHitPoint = this.gameObject.transform.position;

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
    /// フックの初期化処理
    /// </summary>
    public void Initialize(Vector3 startPosition, Vector3 direction, float maxDistance)
    {
        startPoint.position = startPosition;
        endPoint.position = startPosition;
        // hookTarget = startPosition + direction.normalized * maxDistance; // フックの最大到達点を設定
        isExtending = true;
        isRetracting = false;
    }

    /// <summary>
    /// フックを伸ばす処理を開始する処理
    /// </summary>
    /// <param name="direction">フックを飛ばすベクトル方向</param>
    public void StartExtending(Vector2 direction)
    {
        // フックを伸ばす処理を開始する
        isExtending = true;

        // 縮める処理を無効化する
        isRetracting = false;

        // フックが引っかかっていない状態にする
        isHooked = false;

        // フックのターゲット
        // targetPoint = (Vector2)basePoint.position + direction.normalized * maxLength;
    }

    /// <summary>
    /// フックを縮める処理を開始する処理
    /// </summary>
    public void StopExtending()
    {
        // フックを伸ばす処理を無効化する
        isExtending = false;

        // フックを縮める
        isRetracting = true;
    }

    /// <summary>
    /// フックを伸ばす処理
    /// </summary>
    private void ExtendHook()
    {
        // endPoint.position = Vector3.MoveTowards(endPoint.position, hookTarget, extendSpeed * Time.deltaTime);

        // // フックが特定のオブジェクトに当たった場合
        // if (Physics2D.OverlapPoint(endPoint.position, hookableLayers))
        // {
        //     isExtending = false;
        //     hookJump.OnHookAttached(endPoint.position); // HookJumpに通知
        // }
    }

    public void StartRetracting()
    {
        isExtending = false;
        isRetracting = true;
    }

    // フックを縮める処理
    private void RetractHook()
    {
        // プレイヤーの位置を取得
        endPoint.position = Vector3.MoveTowards(endPoint.position, startPoint.position, retractSpeed * Time.deltaTime);

        if (Vector3.Distance(endPoint.position, startPoint.position) < 0.1f)
        {
            isRetracting = false;
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


    // フックが引っかかっている地点を取得できるメソッド
    public Vector2 GetHookedPosition()
    {
        return targetPoint;
    }
}
