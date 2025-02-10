using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookJump : MonoBehaviour
{
    // 基本アクション用フィールド
    [SerializeField] private float MoveSpeed = 7;    // 移動速度
    [SerializeField] private float JumpForce = 7;    // ジャンプ力
    [SerializeField] private GameObject GroundCheck; // 地面チェック用オブジェクト
    [SerializeField] private LayerMask GroundLayer;  // 地面判定用レイヤー
    private bool _isGround;                          // 地面にいるかどうか
    private Rigidbody2D _rigidbody2d;


    // 円周上でオブジェクトを移動させる用フィールド
    [SerializeField] private GameObject ObjectB;  // 円周上に配置するオブジェクト
    [SerializeField] private float radius = 2.0f; // 円の半径
    [SerializeField] private int segments = 100;  // 円をギズモ表示する線の分割数


    // マウスの方向にオブジェクトを伸縮させる用フィールド
    [SerializeField] private GameObject LongShotPrefab;     // 長く伸ばしたいオブジェクト(StretchableObjectがアタッチ)

    /*-----StretchableObjectのフィールドに代入する変数--------------------------------*/
    [SerializeField] private float StretchSpeed = 5.0f;     // オブジェクトの伸びる速度
    [SerializeField] private float RetractSpeed = 5.0f;     // オブジェクトの縮む速度
    [SerializeField] private float MaxStretchLength = 3.0f; // オブジェクトの長さの上限
    [SerializeField] private float HoldTime = 0.5f;         // 最大まで伸びた後の待機時間
    /*------------------------------------------------------------------------------*/
    private Camera _camera;                                 // マウス座標取得用カメラ


    // フックを飛ばす用フィールド
    [SerializeField] private float hookSpeed = 20f; // フックの速度
    [SerializeField] private float moveSpeed = 30f; // プレイヤーの移動速度


    // 壁張り付き用フィールド
    [SerializeField] private GameObject WallCheck; // 前方の壁チェック用オブジェクト
    private bool _isWallMounted; // 壁取得判定

    private GameObject currentStretchableObject;
    private GameObject currentHook;
    private bool isMoving = false;
    private Vector3 targetPosition;

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
    }

    void Update()
    {
        // 基本移動＆ジャンプ
        PlayerMoveInput();

        // プレハブ生成位置をマウスの方向に移動
        RoundMoveObject();

        if (Input.GetMouseButtonDown(0) && currentStretchableObject == null)
        {
            // オブジェクトをオブジェクトBからマウスの方向に伸び縮みさせる
            ShootStretchableObject();
        }

        Debug.Log(isMoving);

        if (isMoving)
        {
            Debug.Log("HitChack");
            MoveToHook();
        }
    }

    /// <summary>
    /// オブジェクトを円周上で移動
    /// </summary>
    private void RoundMoveObject()
    {
        // マウス座標取得（ワールド座標に変換）
        Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; // 2D環境の場合、z軸を0に固定

        // 中心オブジェクトAの座標（このスクリプトがアタッチされているオブジェクト）
        Vector3 centerPos = transform.position;

        // A → マウス座標の方向ベクトル
        Vector2 direction = mousePos - centerPos;

        // 角度を取得
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 角度から円周上の座標を計算
        Vector3 newPos = new Vector3(
            centerPos.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad),
            centerPos.y + radius * Mathf.Sin(angle * Mathf.Deg2Rad),
            0
        );

        // オブジェクトBを移動
        ObjectB.transform.position = newPos;

        // オブジェクトBの向きをマウスがある方向に向ける
        ObjectB.transform.rotation = Quaternion.Euler(ObjectB.transform.rotation.x, ObjectB.transform.rotation.y, angle - 90);
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
    /// 伸縮するオブジェクトを生成するスクリプト
    /// </summary>
    private void ShootStretchableObject()
    {
        Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // 方向を計算
        Vector3 direction = (mousePos - ObjectB.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 伸びるオブジェクトを生成し、方向をセット
        currentStretchableObject  = Instantiate(LongShotPrefab, ObjectB.transform.position, Quaternion.Euler(0, 0, angle));

        // `StretchableLine`を取得
        StretchableLine stretchable = currentStretchableObject.GetComponent<StretchableLine>();
        if (stretchable != null)
        {
            // `StretchableLine`に数値を渡す
            stretchable.Initialize(MaxStretchLength, StretchSpeed, RetractSpeed, HoldTime);
        }
    }


    /// <summary>
    /// フックがヒットしたら移動開始
    /// </summary>
    public void OnHookHit(Vector3 hitPosition)
    {
        if (currentStretchableObject != null)
        {
            targetPosition = hitPosition;
            isMoving = true;
        }
    }
    /// <summary>
    /// プレイヤーをフックの位置へ移動
    /// </summary>
    private void MoveToHook()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        Debug.Log("HookJump");

        // フックの当たった位置に近づいたら移動を解除
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            HookForceJump();
            isMoving = false;
            Destroy(currentStretchableObject);
            currentStretchableObject = null;  // 変数をリセット
        }
    }



    /// <summary>
    /// 基本移動能力管理メソッド
    /// </summary>
    private void PlayerMoveInput()
    {
        // 地面判定取得
        CheckGround();

        // if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        // {
        //     Invoke(nameof(ResetJump), 0f);
        // }

        if (!isMoving) // Hookジャンプ中は操作を無効化
        {
            // 左右移動
            if (Input.GetKey(KeyCode.A))
            {
                SideMove(Vector2.left);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                SideMove(Vector2.right);
            }
            else // 操作していなければ停止
            {
                SideMove(Vector2.zero);
            }
        }

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
        _rigidbody2d.velocity = new Vector2(direction.x * MoveSpeed, _rigidbody2d.velocity.y);
        Direction(direction);
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
            _rigidbody2d.velocity = new Vector2(_rigidbody2d.velocity.x, JumpForce);
        }
    }

    private void HookForceJump()
    {
        _rigidbody2d.velocity = new Vector2(_rigidbody2d.velocity.x, JumpForce);
    }

    /// <summary>
    /// 地面判定取得
    /// </summary>
    private void CheckGround()
    {
        float rayLength = 0.5f;
        _isGround = Physics2D.Raycast(GroundCheck.transform.position, Vector2.down, rayLength, GroundLayer);

        // if(isJumping)
        // {
        //     isJumping = !Physics2D.Raycast(GroundCheck.transform.position, Vector2.down, rayLength, GroundLayer);
        // }


        Debug.Log($"ChackGround : {_isGround}");
        // レイを表示（緑で表示、判定取得で赤で表示）
        Debug.DrawRay(GroundCheck.transform.position, Vector2.down * rayLength, _isGround ? Color.green : Color.red);
    }
}

