using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;

public class HookJump : MonoBehaviour
{
    // 基本アクション用フィールド
    [SerializeField] private float MoveForce = 7f;        // 移動力
    [SerializeField] private float MoveSpeedMax = 5f;     // 最大移動速度
    [SerializeField] private float JumpForce = 7f;        // ジャンプ力
    [SerializeField] private float BrakeStrength = 3.5f;  // ブレーキの力
    [SerializeField] private GameObject GroundCheck;  // 地面チェック用オブジェクト
    [SerializeField] private LayerMask GroundLayer;   // 地面判定用レイヤー
    private bool _isGround;                           // 地面にいるかどうか
    private bool _isPerformingAction = false;         // 他のアクション中かどうか
    private Rigidbody2D _rigidbody2d;
    private Coroutine _brakeCoroutine;


    // 円周上で移動先標示を移動させる用フィールド
    [SerializeField] private GameObject ObjectB;  // 円周上に配置する移動先標示
    [SerializeField] private float radius = 2.0f; // 円の半径
    [SerializeField] private int segments = 100;  // 円をギズモ表示する線の分割数
    [SerializeField] private float sizeMagnificationMax = 2f;   // 移動先標示最大拡大倍率
    [SerializeField] private float sizeMagnificationMin = 0.1f; // 移動先標示最小拡大倍率

    private Vector3 arrowOriginalSize;                          // 移動先標示の元のサイズ


    // マウスの方向にオブジェクトを伸縮させる用フィールド
    [SerializeField] private GameObject LongShotPrefab;     // 長く伸ばしたいオブジェクト(StretchableObjectがアタッチ)

    /// <summary>
    ///  フックを移動させる用フィールド
    /// </summary>
    public GameObject hookPrefab;               // 事前にアタッチされた StretchableLine_2
    public Transform firePoint;
    public float HookLengthMax = 10f;           // フックを伸ばせる長さの上限
    public float HookLengthExpansion = 7.5f;    // フックが命中してから長さ上限に加算される長さ
    public float retractSpeed = 15f;            // フックを伸ばす速度
    public float rewindSpeed = 20f;             // フックを巻き戻す速度


    private GameObject currentHook;             // 処理に利用するフックのオブジェクト
    private Vector3 hookTargetPosition;         // フックが命中した位置
    private float initialDistance;              // 現在のプレイヤーとフックの距離

    public float jumpSpeedmultiplier = 1.5f;    // フックジャンプ時の速度
    private bool isHookFired = false;           // フックが発射されているか判定
    private bool isStartHookFired = false;      // フックの最初の移動が続いているか判定
    private bool isHookRewind = false;          // フックが巻き戻っているか判定
    private bool isHookHit = false;             // フックが命中したか判定
    private Physics2D physics2D;

    // プロパティ
    public bool IsHookFired // フックが発射されているか判定
    {
        get { return isHookFired; }  // 参照可
    }

    public bool IsStartHookFired // フックの最初の移動が続いているか判定
    {
        get { return isStartHookFired; }  // 参照可
    }

    public bool IsHookRewind // フックが巻き戻っているか判定
    {
        get { return isHookRewind; }  // 参照可
    }

    public bool IsHookHit // フックが命中したか判定
    {
        get { return isHookHit; }  // 参照可
        set { isHookHit = value; } // 変更可
    }

    public Vector3 HookTargetPosition // フックが命中した位置
    {
        get { return hookTargetPosition; }  // 参照可
        set { hookTargetPosition = value; } // 変更可
    }



    /*------------------------------------------------------------------------------*/
    private Camera _camera;                     // マウス座標取得用カメラ
    private Vector3 currentMousePos;            // 現在のマウスの位置


    // フックを飛ばす用フィールド
    [SerializeField] private float hookSpeed = 20f; // フックの速度
    [SerializeField] private float moveSpeed = 30f; // プレイヤーの移動速度


    // 壁張り付き用フィールド
    [SerializeField] private GameObject WallCheck; // 前方の壁チェック用オブジェクト
    private bool _isWallMounted; // 壁取得判定

    // private GameObject currentStretchableObject;
    // private GameObject currentHook;
    // private bool isMoving = false;
    // private Vector3 targetPosition;

    void Awake()
    {
        _rigidbody2d = GetComponent<Rigidbody2D>();
        if (GroundCheck == null)
        {
            GroundCheck = gameObject;
        }
        _camera = Camera.main;

        // // 物理マテリアルの数値を変更
        // PhysicsMaterial2D material = _rigidbody2d.sharedMaterial;
        // material.friction = 0.005f;
        // material.bounciness = 0f;

        if (hookPrefab != null)
        {
            currentHook = hookPrefab;
            currentHook.SetActive(false);
        }
    }

    void Start()
    {
        // 移動先標示のオブジェクトのサイズを保存
        arrowOriginalSize = ObjectB.transform.localScale;

        // 移動先標示を非アクティブ
        ObjectB.SetActive(false);
    }

    void Update()
    {
        // 基本移動＆ジャンプ
        PlayerMoveInput();

        // レイを飛ばして判定を取る
        RopePhy();

        // フックが命中していなければフック移動
        if (isHookHit == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // 巻き戻しが終わっていなければ
                if (isHookRewind)
                {
                    // フックの巻き戻し処理終了判定
                    isHookRewind = false;
                }

                // マウスの位置取得開始
                currentMousePos = GetMousePos();

                // フックをアクティブ化
                SetActiveHook(true);

                // フックをマウスの位置まで移動開始
                FireHook();
            }
            else if (Input.GetMouseButton(0))
            {
                // マウスの位置を継続取得
                currentMousePos = GetMousePos();

                // 最初のフック移動が終了したら、フックが命中していなければ
                if (isStartHookFired == false && isHookHit == false)
                {
                    // プレイヤーとマウスの距離がフックの最大の長さを超えていれば
                    if (GetPleyerMouseDistance() >= HookLengthMax)
                    {
                        // 最初のフック移動が終了したときの長さまでフックの位置を補正
                        currentHook.transform.position = GetPointOnLine(this.transform.position,
                                                                            currentMousePos,
                                                                            initialDistance);
                    }
                    // プレイヤーとマウスの距離がフックの最大の長さより短くなっていれば
                    else if (GetPleyerMouseDistance() < HookLengthMax)
                    {
                        // マウスの位置までフックを移動させる処理を再開
                        FireHook();
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                // 最初のフック移動が継続されていれば停止させる
                if (isStartHookFired)
                {
                    isStartHookFired = false;
                }

                // フックをプレイヤーの位置まで戻す
                HookRewind();
            }
        }


        // フックが何かに命中したら
        if (isHookHit)
        {
            /* この時点でフックが命中した位置を取得が完了(StretchableLine_2から変更される) */

            // 最初のフック移動が継続されていれば停止させる
            if (isStartHookFired)
            {
                isStartHookFired = false;
            }

            // 命中地点までの距離が長さの上限に達していれば
            if (GetPlayerTargetDistance() >= HookLengthMax + HookLengthExpansion)
            {
                // 取得した位置をリセット
                hookTargetPosition = new Vector3(0, 0, 0);

                // フックをプレイヤーの位置まで戻す
                HookRewind();

                // 移動先標示を非アクティブ
                ObjectB.SetActive(false);

                // フックの命中状態を解除する
                isHookHit = false;
            }

            // フックの位置を命中した地点で固定
            currentHook.transform.position = hookTargetPosition;

            // 移動先を矢印で表示
            RoundMoveObject();

            if (Input.GetMouseButtonUp(0))
            {
                // 命中した地点の長さに応じた強さのジャンプをする
                HookPowerJump(GetTwoPointNormalized(this.transform.position, hookTargetPosition));

                // 取得した位置をリセット
                hookTargetPosition = new Vector3(0, 0, 0);

                // フックをプレイヤーの位置まで戻す
                HookRewind();

                // 移動先標示を非アクティブ
                ObjectB.SetActive(false);

                // フックの命中状態を解除する
                isHookHit = false;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                // 取得した位置をリセット
                hookTargetPosition = new Vector3(0, 0, 0);

                // フックをプレイヤーの位置まで戻す
                HookRewind();

                // 移動先標示を非アクティブ
                ObjectB.SetActive(false);

                // フックの命中状態を解除する
                isHookHit = false;
            }
        }
    }

    /// <summary>
    /// オブジェクトを円周上で移動
    /// </summary>

    // フックジャンプの移動先を円周上の矢印の方向、移動量を矢印の大きさで表現する
    private void RoundMoveObject()
    {
        // フックが命中していなければ処理は行わない
        if (isHookHit == false) return;

        // 移動先標示をアクティブ化
        ObjectB.SetActive(true);

        // 中心オブジェクトAの座標（このスクリプトがアタッチされているオブジェクト）
        Vector3 centerPos = transform.position;

        // プレイヤーと命中地点の距離を計算
        float distance = GetPlayerTargetDistance();

        // ジャンプで移動する方向ベクトル
        Vector2 direction = JumpVectorForce(
                            GetTwoPointNormalized(this.transform.position, hookTargetPosition),
                            distance,
                            jumpSpeedmultiplier);

        // 角度を取得
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 角度から円周上の座標を計算
        Vector3 newPos = new Vector3(
            centerPos.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad),
            centerPos.y + radius * Mathf.Sin(angle * Mathf.Deg2Rad),
            0
        );

        // 移動先標示のサイズを距離に応じて変更
        ArrowObjectSizeChange(distance);

        // 移動先標示を移動
        ObjectB.transform.position = newPos;

        // 移動先標示の向きを計算した角度の方向に向ける
        ObjectB.transform.rotation = Quaternion.Euler(ObjectB.transform.rotation.x, ObjectB.transform.rotation.y, angle - 90);
    }

    // 移動先標示のサイズを距離に応じて変更
    private void ArrowObjectSizeChange(float distance)
    {
        // 距離を拡大倍率に変換
        float sizeMagnification = distance / 5f;

        // 拡大倍率が最大値以上なら最大値に変更
        if (sizeMagnification >= sizeMagnificationMax)
        {
            sizeMagnification = sizeMagnificationMax;
        }
        // 拡大倍率が最小値以下なら最小値に変更
        else if (sizeMagnification <= sizeMagnificationMin)
        {
            sizeMagnification = sizeMagnificationMin;
        }

        // 移動先標示のサイズを距離に応じたサイズに変更
        ObjectB.transform.localScale = new Vector3(arrowOriginalSize.x * sizeMagnification,
                                                   arrowOriginalSize.y * sizeMagnification,
                                                   arrowOriginalSize.z);
    }

    /// <summary>
    /// 円のギズモ表示
    /// </summary>
    private void OnDrawGizmos()
    {
        if (segments < 3) return; // 最低3点必要（多角形になるため）

        Gizmos.color = Color.green; // 円の色を緑に設定
        Vector3 centerPos = transform.position;
        float angleStep = 360f / segments;

        Vector3 prevPoint = centerPos + new Vector3(radius, 0, 0); // 初期点

        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i;
            Vector3 newPoint = new Vector3(
                centerPos.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad),
                centerPos.y + radius * Mathf.Sin(angle * Mathf.Deg2Rad),
                0
            );

            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }

    /// <summary>
    /// ロープの地面判定取得
    /// </summary>
    private void RopePhy()
    {
        // フックまでの距離を取得
        float rayLength = GetPlayerHookDistance();

        // フックが命中したかどうか判定
        bool isRopeHit = Physics2D.Raycast(this.transform.position,
                                           GetTwoPointNormalized(this.transform.position, currentHook.transform.position),
                                           rayLength,
                                           GroundLayer);

        Debug.Log($"isRopeHit : {isRopeHit}");

        // レイを表示（緑で表示、判定取得で赤で表示）
        Debug.DrawRay(this.transform.position,
                      GetTwoPointNormalized(this.transform.position, currentHook.transform.position) * rayLength,
                      isRopeHit ? Color.green : Color.red);
    }

    /// <summary>
    /// マウスの位置取得
    /// </summary>
    /// <returns></returns>
    private Vector3 GetMousePos()
    {
        Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }

    /// <summary>
    /// フックのアクティブ状態変更
    /// </summary>
    /// <param name="setflag"></param>
    private void SetActiveHook(bool setflag)
    {
        currentHook.SetActive(setflag);
    }

    /// <summary>
    /// フックのアクティブ状態確認
    /// </summary>
    /// <returns></returns>
    private bool ActiveChackHook()
    {
        return currentHook.activeInHierarchy;
    }

    /// <summary>
    /// フックを移動させる
    /// </summary>
    private void FireHook()
    {
        StartCoroutine(FireHookIEnumerator());
    }
    /// <summary>
    /// フック移動の内部処理コルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator FireHookIEnumerator()
    {
        // フックがアクティブではなく、最初のフック移動が既に始まっていれば、処理を強制終了
        if (ActiveChackHook() == false && isStartHookFired) yield break;

        Debug.Log("FireHookMove ON");

        // フックが発射中
        isHookFired = true;

        // 最初のフックの移動が開始
        isStartHookFired = true;

        // プレイヤーとマウスの直線上にフックの位置を初期化
        Vector3 InitializationPos = GetPointOnLine(this.transform.position, currentMousePos, 0f);
        currentHook.transform.position = InitializationPos;

        // フックとマウスの距離が近づくまで、プレイヤーとフックの距離が限界距離になるまでループ
        while (GetPlayerHookDistance() <= HookLengthMax)
        {
            // 最初のフック移動判定が切り替わることがあれば処理を強制終了
            if (isStartHookFired == false) yield break;

            // 現在のプレイヤーとマウスの直線上で、initialDistanceだけ進んだ位置にフックの位置を補正
            Vector3 targetPos = GetPointOnLine(this.transform.position, currentMousePos, initialDistance);
            currentHook.GetComponent<StretchableLine_2>().MoveTo(targetPos);

            // 進んだ後のプレイヤーとフックの距離を記録
            initialDistance = GetPlayerHookDistance();

            Debug.Log("FireHookMove Update");

            // Whileの最初に戻る
            yield return null;
        }

        // 移動終了時のプレイヤーとフックの距離を記録
        initialDistance = GetPlayerHookDistance();

        // 最初のフックの移動が終了
        isStartHookFired = false;
    }

    /// <summary>
    /// フックをプレイヤーの位置まで巻き戻す
    /// </summary>
    private void HookRewind()
    {
        StartCoroutine(RewindHookIEnumerator());
    }
    /// <summary>
    /// フックをプレイヤーの位置まで巻き戻す内部処理コルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator RewindHookIEnumerator()
    {
        // フックがアクティブでなければ処理を強制終了
        if (ActiveChackHook() == false) yield break;

        Debug.Log("RewindHookMove ON");

        // 現在のプレイヤーとフックの距離を記録
        initialDistance = GetPlayerHookDistance();

        // フックの巻き戻し処理開始判定
        isHookRewind = true;

        // プレイヤーとフックの距離が近づくまでループ
        while (GetPlayerHookDistance() >= 0.1f)
        {
            // フック移動が再開されたら処理を強制終了
            if (isStartHookFired == true)
            {
                // フックの巻き戻し処理終了判定
                isHookRewind = false;
                yield break;
            }

            // 現在のプレイヤーとマウスの直線上で、initialDistanceだけ戻った位置にフックの位置を補正
            Vector3 targetPos = GetPointOnLine(this.transform.position, currentMousePos, initialDistance);
            currentHook.GetComponent<StretchableLine_2>().MoveTo(targetPos);

            // 戻った後のプレイヤーとフックの距離を記録
            initialDistance = GetPlayerHookDistance();

            Debug.Log("RewindHookMove Update");

            // Whileの最初に戻る
            yield return null;
        }

        // フックの巻き戻し処理終了判定
        isHookRewind = false;

        // プレイヤーとフックの距離をリセット
        initialDistance = 0f;

        // フックの発射が終了
        isHookFired = false;

        // フックを非アクティブにする
        SetActiveHook(false);
    }

    /// <summary>
    /// プレイヤーとフックの距離を計算するメソッド
    /// </summary>
    /// <returns></returns>
    private float GetPlayerHookDistance()
    {
        Vector3 playerPos = this.transform.position;           // プレイヤーの位置を取得
        Vector3 hookPointPos = currentHook.transform.position; // フックの位置を取得
        return Vector3.Distance(playerPos, hookPointPos);      // プレイヤーとフックの距離を計算
    }

    /// <summary>
    /// プレイヤーとフック命中地点の距離を計算するメソッド
    /// </summary>
    /// <returns></returns>
    private float GetPlayerTargetDistance()
    {
        Vector3 playerPos = this.transform.position;            // プレイヤーの位置を取得
        return Vector3.Distance(playerPos, hookTargetPosition); // プレイヤーと命中地点の距離を計算
    }

    /// <summary>
    /// プレイヤーとマウスの距離を計算するメソッド
    /// </summary>
    /// <returns></returns>
    private float GetPleyerMouseDistance()
    {
        Vector3 playerPos = this.transform.position;       // プレイヤーの位置を取得
        Vector3 mousePointPos = currentMousePos;           // マウスの位置を取得
        return Vector3.Distance(playerPos, mousePointPos); // プレイヤーとマウスの距離を計算
    }

    /// <summary>
    /// マウスとフックの距離を計算するメソッド
    /// </summary>
    /// <returns></returns>
    private float GetMouseHookDistance()
    {
        Vector3 mousePointPos = currentMousePos;               // マウスの位置を取得
        Vector3 hookPointPos = currentHook.transform.position; // フックの位置を取得
        return Vector3.Distance(mousePointPos, hookPointPos);  // マウスとフックの距離を計算
    }

    /// <summary
    /// firstPos から secondPos まで向かうベクトル方向を求める
    /// </summary>
    /// <param name="firstPos"></param>
    /// <param name="endPos"></param>
    /// <returns></returns>
    private Vector2 GetTwoPointNormalized(Vector3 firstPos, Vector3 secondPos)
    {
        Vector2 vector = (secondPos - firstPos).normalized;
        return vector;
    }

    /// <summary>
    /// A から B に向かって d の距離だけ進んだ点を求める
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <param name="d"></param>
    /// <returns></returns>
    public static Vector2 GetPointOnLine(Vector2 A, Vector2 B, float d)
    {
        Vector2 direction = (B - A).normalized; // 単位ベクトル
        return A + direction * d;
    }

    /// <summary>
    /// 指定した方向にプレイヤーと命中地点の距離に応じた力を加えてジャンプ
    /// </summary>
    /// <param name="direciton"></param>
    private void HookPowerJump(Vector2 direciton)
    {
        // 移動を伴うアクションのフラグを開始
        MoveActionFlag(0.1f);

        // ブレーキ中であれば
        if (_brakeCoroutine != null)
        {
            // ブレーキ処理を止める
            StopCoroutine(_brakeCoroutine);
        }
        // 現在のプレイヤーと命中地点の距離を取得
        float Distance = GetPlayerTargetDistance();

        // 現在の速度をリセット
        _rigidbody2d.velocity = new Vector2(0, 0);

        // 指定した方向に瞬間的に力を加えてジャンプ
        _rigidbody2d.AddForce(direciton * (Distance * jumpSpeedmultiplier), ForceMode2D.Impulse);
    }

    /// <summary>
    /// ベクトル方向と距離と力を指定してジャンプするベクトル方向と大きさを求める
    /// </summary>
    /// <param name="direciton"></param>
    /// <param name="distance"></param>
    /// <param name="jumpForce"></param>
    /// <returns></returns>
    private Vector2 JumpVectorForce(Vector2 direciton, float distance, float jumpForce)
    {
        Vector2 jumpVectorForce = direciton * (distance * jumpForce);
        return jumpVectorForce;
    }


    /// <summary>
    /// 基本移動能力管理メソッド
    /// </summary>
    private void PlayerMoveInput()
    {
        // 地面判定取得
        CheckGround();

        Vector3 moveDirection = Vector3.zero;


        // 左右移動
        // A入力がある状態で
        if (Input.GetKey(KeyCode.A))
        {
            // 移動方向を左に指定
            moveDirection = Vector3.left;
        }
        // D入力がある状態で
        else if (Input.GetKey(KeyCode.D))
        {
            // 移動方向を右に指定
            moveDirection = Vector3.right;
        }
        // 地面に立っていて、入力が無いとき
        else if (_isGround && !_isPerformingAction)
        {
            // 速度が一定以上の場合
            if(Mathf.Abs(_rigidbody2d.velocity.x) >= MoveSpeedMax)
            {
                InertiaMoveStop();
            }
            else
            {
                MoveStop();
            }
        }

        // ブレーキ処理
        if (moveDirection != Vector3.zero)
        {
            MoveBrake(moveDirection);
        }

        // 通常移動処理
        SideMove(moveDirection);

        // ジャンプ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GroundJump();
        }

    }

    /// <summary>
    /// プレイヤーの左右移動
    /// </summary>
    /// <param name="direction"></param>
    public void SideMove(Vector3 direction)
    {
        Vector2 velocity = _rigidbody2d.velocity;

        // 現在の速度が最大値を超えていなければ
        if (Mathf.Abs(velocity.x) < MoveSpeedMax)
        {
            // 継続的に力を加えて移動
            _rigidbody2d.AddForce(new Vector2(direction.x * MoveForce, 0), ForceMode2D.Force);
        }

        Direction(direction);
    }

    /// <summary>
    /// 移動を停止する
    /// </summary>
    public void MoveStop()
    {
        _rigidbody2d.velocity = new Vector2(0, _rigidbody2d.velocity.y);
    }

    /// <summary>
    /// 慣性を維持しつつ移動を停止する
    /// </summary>
    public void InertiaMoveStop()
    {
        Vector2 velocity = _rigidbody2d.velocity;
    }

    /// <summary>
    /// 移動を伴うアクションの判定を開始した処理
    /// </summary>
    /// <param name="delay"></param>
    private void MoveActionFlag(float delay)
    {
        StartCoroutine(ResetActionFlag(delay));
    }

    /// <summary>
    /// 移動以外のアクション時間の判定
    /// </summary>
    /// <returns></returns>
    IEnumerator ResetActionFlag(float delay)
    {
        _isPerformingAction = true;
        yield return new WaitForSeconds(delay); // アクションの継続時間に応じて調整
        _isPerformingAction = false;
    }

    /// <summary>
    /// 移動が速い状態で方向転換をするとき徐々に速度を下げる
    /// </summary>
    /// <param name="direction"></param>
    public void MoveBrake(Vector3 direction)
    {
        Vector2 velocity = _rigidbody2d.velocity;

        // 逆方向への入力があり、かつ現在の速度が一定以上ならブレーキ処理開始
        if (Mathf.Sign(velocity.x) != Mathf.Sign(direction.x) && Mathf.Abs(velocity.x) > 0.1f)
        {
            // ブレーキ中であれば
            if (_brakeCoroutine != null)
            {
                // ブレーキ処理を止める
                StopCoroutine(_brakeCoroutine);
            }
            // ブレーキ処理を開始する
            _brakeCoroutine = StartCoroutine(ApplyBrake());
        }
    }

    /// <summary>
    /// 速度を徐々に 0 にする方向転換ブレーキ処理
    /// </summary>
    private IEnumerator ApplyBrake()
    {
        // ブレーキをかける前の方向を保存
        float initialDirection = Mathf.Sign(_rigidbody2d.velocity.x);

        // ブレーキに掛かる時間が過ぎるまで
        while (Mathf.Abs(_rigidbody2d.velocity.x) > 0.1f) // 速度がほぼ0になるまで
        {
            Debug.Log("brake");
            // 現在の速度を取得
            Vector2 velocity = _rigidbody2d.velocity;

            // 現在の速度の方向を取得
            float currentDirection = Mathf.Sign(velocity.x);

            // 方向が反転したらブレーキ終了
            if (currentDirection != initialDirection)
            {
                break;
            }

            // **新しい入力方向を確認**
            float input = 0;
            if (Input.GetKey(KeyCode.A)) input = -1;
            if (Input.GetKey(KeyCode.D)) input = 1;

            // **新しい入力があればブレーキ終了**
            if (input != 0 && Mathf.Sign(input) != Mathf.Sign(initialDirection))
            {
                Debug.Log("新しい入力を検知、ブレーキ解除");
                break;
            }

            // 速度の向きとは逆方向にブレーキ力を加える
            float brakeForce = Mathf.Abs(velocity.x) * BrakeStrength; // 速度に比例した減速力
            _rigidbody2d.AddForce(new Vector2(-currentDirection * brakeForce, 0), ForceMode2D.Force);

            yield return null;
        }

        // 完全に停止
        _rigidbody2d.velocity = new Vector2(0, _rigidbody2d.velocity.y);
        _brakeCoroutine = null;
    }


    /// <summary>
    /// 向き変更
    /// </summary>
    /// <param name="direction"></param>
    public void Direction(Vector3 direction)
    {
        // 入力がない場合は処理を終了
        if (direction.x == 0) return;

        // Y軸の回転を変更（右向きが0度、左向きが180度）
        float rotationY = direction.x > 0 ? 0 : 180;
        transform.rotation = Quaternion.Euler(0, rotationY, 0);
    }

    /// <summary>
    /// ジャンプ処理
    /// </summary>
    public void GroundJump()
    {
        if (_isGround)
        {
            Debug.Log("Jump");

            // 既存の上下速度をリセット
            _rigidbody2d.velocity = new Vector2(_rigidbody2d.velocity.x, 0);

            // 瞬間的に力を加えてジャンプ
            _rigidbody2d.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
        }
    }

    /// <summary>
    /// 地面判定取得
    /// </summary>
    private void CheckGround()
    {
        float rayLength = 0.5f;
        _isGround = Physics2D.Raycast(GroundCheck.transform.position, Vector2.down, rayLength, GroundLayer);

        Debug.Log($"ChackGround : {_isGround}");
        // レイを表示（緑で表示、判定取得で赤で表示）
        Debug.DrawRay(GroundCheck.transform.position, Vector2.down * rayLength, _isGround ? Color.green : Color.red);
    }
}

