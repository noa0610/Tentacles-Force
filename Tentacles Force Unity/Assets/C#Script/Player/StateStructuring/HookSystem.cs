using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/*
 * フックシステムのクラス
 *
 * フックを使って敵や地面に引っ掛ける機能を管理します。
 * 設定されたプレハブをフックとして生成し、マウスの位置に合わせて移動させます。
 * フックの発射、巻き戻し、ロープの描画、当たり判定などが行えます。
 *
 * 列挙型でフックの状態を管理しており、
 * Idle、Fire、Rewind、Hitの4つの状態に分かれています。
 */

public class HookSystem : MonoBehaviour
{
    [SerializeField] private GameObject HookPrefab; // フックのプレハブ
    [SerializeField] private GameObject HookFirePoint; // フックの発射位置
    [SerializeField] private float HookLengthMax = 10f; // フックの最大距離
    [SerializeField] private float HookHitLengthMax = 15f; // フックのヒット時の最大距離
    [SerializeField] private float RetractSpeed = 15f; // フックの伸ばす速度
    [SerializeField] private float RewindSpeed = 20f; // フックの巻き戻し速度
    [SerializeField] private LayerMask GroundLayer; // 地面のレイヤー
    [SerializeField] private LayerMask EnemyLayer; // 敵のレイヤー
    [SerializeField] private Material RopeMaterial; // ロープのマテリアル
    [SerializeField] private float RopeWidth = 0.3f; // ロープの太さ

    private LineRenderer ropeRenderer; // フックのロープの描画
    private GameObject hook;
    private Vector3 currentMousePos;    // マウスの位置
    private Vector3 hookTargetPosition; // フックのターゲット位置
    private GameObject enemy;
    private bool isHookMaxLength = false;
    private bool isHookHitMaxLength = false;
    private float initialDistance;
    private Rigidbody2D _rb2DHook;
    private SpriteRenderer _sprRenHook;
    private Vector3 firstFirePoint; // 初期のフックの発射位置

    public Vector3 HookTargetPosition { get; set; }

    public enum HookState
    {
        Idle,
        Fire,
        Rewind,
        Hit,
        EnemyHit
    }
    public HookState currentHookState = HookState.Idle;

    private void Awake()
    {
        if (HookFirePoint == null)
        {
            HookFirePoint = this.gameObject; // フックの発射位置をこのゲームオブジェクトに設定
        }

        firstFirePoint = HookFirePoint.transform.position; // 初期のフックの発射位置を記録

        // 初期化処理
        if (HookPrefab != null)
        {
            // フックを生成
            hook = Instantiate(HookPrefab, HookFirePoint.transform.position, Quaternion.identity);
            hook.transform.position = HookFirePoint.transform.position;

            _sprRenHook = hook.GetComponent<SpriteRenderer>();

            _rb2DHook = hook.GetComponent<Rigidbody2D>();
            _rb2DHook.isKinematic = true;
            _rb2DHook.gravityScale = 0;

            ropeRenderer = hook.GetComponent<LineRenderer>();
            ropeRenderer.positionCount = 2; // ロープの頂点数を2に設定
            ropeRenderer.startWidth = RopeWidth; // ロープの太さを設定
            ropeRenderer.endWidth = RopeWidth;   // ロープの太さを設定
            ropeRenderer.material = RopeMaterial; // ロープのマテリアルを設定
            ropeRenderer.textureMode = LineTextureMode.Tile; // テクスチャーモードをタイルに設定

            hook.SetActive(false); // フックを非アクティブ化
        }
        else
        {
            Debug.LogError("Hook prefab is not assigned in the inspector. フックプレハブがアサインされていません。");
        }
    }

    // ================================================================================ 
    // フック発射、巻き戻し、マウス追従の処理
    // ================================================================================ 

    /// <summary>
    /// フックを発射する処理
    /// </summary>
    public void StartFire()
    {
        // フックをアクティブ化
        hook.SetActive(true);

        //　マウスの位置を取得
        currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentMousePos.z = 0;

        StartCoroutine(HookFireCoroutine());
    }
    private IEnumerator HookFireCoroutine()
    {
        currentHookState = HookState.Fire;

        isHookMaxLength = false;

        // フックを発射する処理
        initialDistance = Vector2.Distance(hook.transform.position, HookFirePoint.transform.position);

        // フックとマウスの距離が近づくまでループ
        while (GetHookDistance() <= HookLengthMax)
        {
            if (currentHookState != HookState.Fire) yield break;

            // マウスの位置を取得
            currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentMousePos.z = 0;

            // 発射位置からフックの位置までのベクトルを計算
            Vector3 direction = (currentMousePos - HookFirePoint.transform.position).normalized;

            // 発射位置とマウスの直線上にフックの位置を補正
            hook.transform.position = HookFirePoint.transform.position + direction * initialDistance;

            // フックの位置をマウスの位置に近づける
            hook.transform.position = Vector3.MoveTowards(hook.transform.position, currentMousePos, RetractSpeed * Time.deltaTime);

            // 進んだ後の発射位置とフックの距離を記録
            initialDistance = GetHookDistance();

            HookRotation();     // マウス方向を向かせる回転
            RopeLineRenderer(); // ロープを描画

            // Whileの最初に戻る
            yield return null;
        }

        // 移動終了時の発射位置とフックの距離を記録
        initialDistance = GetHookDistance();

        // フックの最大距離に達したことを記録
        isHookMaxLength = true;

        yield return null;
    }

    /// <summary>
    /// フックを巻き戻す処理
    /// </summary>
    public void StartRewind()
    {
        // 現在のプレイヤーとフックの距離を記録
        initialDistance = GetHookDistance();

        StartCoroutine(HookRewindCoroutine());
    }

    private IEnumerator HookRewindCoroutine()
    {
        currentHookState = HookState.Rewind;

        isHookMaxLength = false;

        // フックを巻き戻す処理
        while (GetHookDistance() > 0.1f)
        {
            // 最初のフック移動判定が切り替わることがあれば処理を強制終了
            if (currentHookState != HookState.Rewind) yield break;

            // 発射位置からフックの位置までのベクトルを計算
            Vector3 direction = (currentMousePos - HookFirePoint.transform.position).normalized;

            // 発射位置とマウスの直線上で、initialDistanceだけ進んだ位置にフックの位置を補正
            hook.transform.position = HookFirePoint.transform.position + direction * initialDistance;

            // フックの位置を発射位置に近づける
            hook.transform.position = Vector3.MoveTowards(hook.transform.position, HookFirePoint.transform.position, RewindSpeed * Time.deltaTime);

            // 進んだ後の発射位置とフックの距離を記録
            initialDistance = GetHookDistance();

            HookRotation();
            RopeLineRenderer();

            yield return null;
        }

        initialDistance = 0; // 巻き戻しが完了したら、フックの位置を発射位置に設定

        currentHookState = HookState.Idle;

        // フックの巻き戻しが完了したら、フックを非アクティブ化
        hook.SetActive(false);
    }

    /// <summary>
    /// フックのマウス追従処理
    /// </summary>
    public void MousePursueUpdate()
    {
        if (isHookMaxLength == false) return; // フックの長さが最大になっていない場合は処理を終了

        // プレイヤーとマウスの距離がフックの最大の長さを超えていれば
        if (GetMouseDistance() >= HookLengthMax)
        {
            currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentMousePos.z = 0;

            Vector3 direction = (currentMousePos - HookFirePoint.transform.position).normalized;

            // 発射位置とマウスの直線上で、記録された距離まで進んだ位置にフックの位置を補正
            hook.transform.position = HookFirePoint.transform.position + direction * initialDistance;

            HookRotation();
            RopeLineRenderer();

        }
        // 発射位置とマウスの距離がフックの最大の長さより短くなっていれば
        else if (GetMouseDistance() < HookLengthMax)
        {
            currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentMousePos.z = 0;
            hook.transform.position = currentMousePos;

            HookRotation();
            RopeLineRenderer();
        }
    }

    /// <summary>
    /// フックがヒットした時の処理
    /// </summary>
    public void HitUpdate()
    {
        if (GetHookDistance() >= HookHitLengthMax)
        { 
            isHookHitMaxLength = true; // フックがヒットした最大距離に達したことを記録
        }

        currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentMousePos.z = 0; // マウスのZ座標を0に設定

        HookRotation();
        RopeLineRenderer();
    }

    public void AttackUpdate()
    {
        hook.transform.position = enemy.transform.position; // フックを敵の位置に合わせる
        HookRotation();
        RopeLineRenderer();
    }


    // フックが発射位置に垂直になるように回転する処理
    private void HookRotation()
    {
        // フックの位置を取得
        Vector3 hookPos = hook.transform.position;

        // 発射位置を取得
        Vector3 firePos = HookFirePoint.transform.position;

        // フックと発射位置のベクトルを計算
        Vector3 direction = (hookPos - firePos).normalized;

        // フックの位置が発射位置より左にある場合
        if (hookPos.x < firePos.x)
        {
            _sprRenHook.flipY = true; // フックのスプライトを反転
        }
        else
        {
            _sprRenHook.flipY = false; // フックのスプライトを元に戻す
        }

        // フックの回転を計算
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // フックの回転を設定
        hook.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    // ================================================================================ 
    // フックの位置計算をする処理（汎用計算用メソッド）
    // ================================================================================ 

    public float GetHookDistance()
    {
        Vector3 firePos = HookFirePoint.transform.position;  // 発射位置を取得
        Vector3 hookPointPos = hook.transform.position;      // フックの位置を取得
        return Vector3.Distance(firePos, hookPointPos);      // 発射位置とフックの距離を計算
    }

    private float GetMouseDistance()
    {
        Vector3 firePos = HookFirePoint.transform.position;  // 発射位置を取得
        Vector3 mousePos = currentMousePos;                  // マウスの位置を取得
        return Vector3.Distance(firePos, mousePos);          // プレイヤーとマウスの距離を計算
    }

    // =================================================================================
    // フックのロープを描画する処理
    // =================================================================================

    /// フックの地点までロープを描画する処理
    private void RopeLineRenderer()
    {
        // ロープの始点
        ropeRenderer.SetPosition(0, HookFirePoint.transform.position);

        // ロープの終点
        ropeRenderer.SetPosition(1, hook.transform.position);
    }

    // ================================================================================
    // フックの当たり判定を管理する処理
    // ================================================================================

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (currentHookState == HookState.Rewind) return;

        // フックが地面に当たった場合
        if (collision.gameObject.layer == GroundLayer)
        {
            // 命中した位置を記録
            hookTargetPosition = collision.transform.position;
            currentHookState = HookState.Hit;
        }
        // フックが敵にあたった場合
        else if (collision.gameObject.layer == EnemyLayer)
        {
            // フックが敵に当たった場合の処理
            enemy = collision.gameObject;
            currentHookState = HookState.EnemyHit;
            Debug.Log("フックが敵に当たりました: " + collision.gameObject.name);
        }
    }

    /// フックのロープがヒットした位置を取得する処理
    public void GetRopeRayPoint()
    {
        if (currentHookState == HookState.Rewind) return;

        int combinedLayerMask = GroundLayer | EnemyLayer;

        // 自分とフックの位置の2点で繋いでレイを飛ばす
        RaycastHit2D hit = Physics2D.Linecast(HookFirePoint.transform.position, hook.transform.position, combinedLayerMask);

        // 命中した場合
        if (hit.collider != null && currentHookState == HookState.Fire)
        {
            // 命中したオブジェクトのレイヤーで分岐
            int hitLayer = hit.collider.gameObject.layer;

            // 地面にヒット
            if ((GroundLayer.value & (1 << hitLayer)) != 0)
            {
                hookTargetPosition = hit.point;
                currentHookState = HookState.Hit;
            }
            // 敵にヒット
            else if ((EnemyLayer.value & (1 << hitLayer)) != 0)
            {
                enemy = hit.collider.gameObject;
                currentHookState = HookState.EnemyHit;
                Debug.Log("敵にヒット: " + hit.collider.gameObject.name);
                // ここで敵への追加処理
            }
        }
    }

    // フックの位置をリセットする処理
    public void ResetPosision(Vector3 otherPos)
    {
        hook.transform.position = otherPos; // フックの位置を元の位置にリセット
    }

    // フックが命中した位置で固定する処理
    public void HitPosisionStay()
    {
        hook.transform.position = hookTargetPosition;
    }

    public Vector2 GetHitPoint()
    {
        return hookTargetPosition;
    }

    public GameObject GetEnemy()
    {
        return enemy;
    }

    public Vector2 GetHookDirection()
    {
        return (hookTargetPosition - transform.position).normalized;
    }

}