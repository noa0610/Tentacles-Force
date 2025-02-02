using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// テンタクルスジャンプテスト用スクリプト
/// </summary>
public class TentaclesJump : MonoBehaviour
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


    // マウスの方向にゲームオブジェクトを飛ばす用フィールド
    [SerializeField] private GameObject Attack;    // プレハブ
    [SerializeField] private float ShotSpeed = 7f; // プレハブを飛ばす速度
    private Camera _camera;                        // マウス座標取得用カメラ


    // マウスの方向にオブジェクトを伸縮させる用フィールド
    [SerializeField] private GameObject LongShotPrefab;     // 長く伸ばしたいオブジェクト(StretchableObjectがアタッチ)

    /*-----StretchableObjectのフィールドに代入する変数--------------------------------*/
    [SerializeField] private float StretchSpeed = 5.0f;     // オブジェクトの伸びる速度
    [SerializeField] private float RetractSpeed = 5.0f;     // オブジェクトの縮む速度
    [SerializeField] private float MaxStretchLength = 3.0f; // オブジェクトの長さの上限
    [SerializeField] private float HoldTime = 0.5f;         // 最大まで伸びた後の待機時間
    /*------------------------------------------------------------------------------*/


    // 伸ばしたオブジェクトの先端と逆方向にプレイヤーが移動する用フィールド
    private bool isJumping = false; // TentacleHookでのジャンプ中かどうか


    void Awake()
    {
        _rigidbody2d = GetComponent<Rigidbody2D>();
        if (GroundCheck == null)
        {
            GroundCheck = gameObject;
        }
        _camera = Camera.main;
    }

    void Update()
    {
        // 基本移動＆ジャンプ
        PlayerMoveInput();

        // プレハブ生成位置をマウスの方向に移動
        RoundMoveObject();


        if (Input.GetKeyDown(KeyCode.E))
        {
            // オブジェクトをオブジェクトBからマウスの方向に発射
            // ShootObject();

            // オブジェクトをオブジェクトBからマウスの方向に伸び縮みさせる
            ShootStretchableObject();
        }
    }

    // オブジェクトを円周上で移動
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

    // 伸縮するオブジェクトを生成するスクリプト
    private void ShootStretchableObject()
    {
        Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // 方向を計算
        Vector3 direction = (mousePos - ObjectB.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 伸びるオブジェクトを生成し、方向をセット
        GameObject instance = Instantiate(LongShotPrefab, ObjectB.transform.position, Quaternion.Euler(0, 0, angle));

        // `StretchableObject` にパラメータを渡して処理開始
        StretchableObject stretchable = instance.GetComponent<StretchableObject>();
        if (stretchable != null)
        {
            stretchable.Initialize(MaxStretchLength, StretchSpeed, RetractSpeed, HoldTime);
        }
    }

    // オブジェクトBの位置の回転を取得してプレハブオブジェクトを発射
    private void ShootObject()
    {
        GameObject instance = Instantiate(Attack,
                                              ObjectB.transform.position,
                                              ObjectB.transform.rotation);

        // オブジェクトBの前方向を取得（Z軸回転 +90 度で上向きと一致）
        float forwardAngle = ObjectB.transform.eulerAngles.z + 90;
        // 弾の移動方向を計算
        Vector2 direction = new Vector2(
            Mathf.Cos(forwardAngle * Mathf.Deg2Rad),
            Mathf.Sin(forwardAngle * Mathf.Deg2Rad))
            .normalized; // ベクトルを単位ベクトル化

        // Rigidbody2Dを取得（なければ追加）
        Rigidbody2D rb = instance.GetComponent<Rigidbody2D>() ?? instance.AddComponent<Rigidbody2D>();

        // 速度を設定
        rb.velocity = direction * ShotSpeed;
        Debug.Log($"rb.velocity : {rb.velocity}");
        Destroy(instance, 2f);
    }

    // 円のギズモ表示
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

    // TentacleHook から呼び出されるメソッド
    public void ApplyTentacleJump(Vector2 jumpForce)
    {
        isJumping = true; // TentacleHookのジャンプ中はX軸移動を無効化
        _rigidbody2d.velocity = Vector2.zero; // 速度リセット（現在の影響を排除）
        _rigidbody2d.AddForce(jumpForce, ForceMode2D.Impulse);
        // Invoke(nameof(ResetJump), 0.3f);
    }

    private void ResetJump()
    {
        isJumping = false; // 通常の移動を再開
    }



    // 基本移動能力管理メソッド
    private void PlayerMoveInput()
    {
        // 地面判定取得
        CheckGround();

        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            Invoke(nameof(ResetJump),0f);
        }

        if (!isJumping) // TentacleHookジャンプ中は操作を無効化
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

    // プレイヤーの左右移動
    public void SideMove(Vector3 direction)
    {
        _rigidbody2d.velocity = new Vector2(direction.x * MoveSpeed, _rigidbody2d.velocity.y);
        Direction(direction);
    }

    // 向き変更
    public void Direction(Vector3 direction)
    {
        // 入力がない場合は処理を終了
        if (direction.x == 0) return;

        // Y軸の回転を変更（右向きが0度、左向きが180度）
        float rotationY = direction.x > 0 ? 0 : 180;
        transform.rotation = Quaternion.Euler(0, rotationY, 0);
    }

    // ジャンプ処理
    public void GroundJump()
    {
        if (_isGround)
        {
            Debug.Log("Jump");
            _rigidbody2d.velocity = new Vector2(_rigidbody2d.velocity.x, JumpForce);
        }
    }

    // 地面判定取得
    private void CheckGround()
    {
        float rayLength = 0.5f;
        _isGround = Physics2D.Raycast(GroundCheck.transform.position, Vector2.down, rayLength, GroundLayer);
        
        if(isJumping)
        {
            isJumping = !Physics2D.Raycast(GroundCheck.transform.position, Vector2.down, rayLength, GroundLayer);
        }
        

        Debug.Log($"ChackGround : {_isGround}");
        // レイを表示（緑で表示、判定取得で赤で表示）
        Debug.DrawRay(GroundCheck.transform.position, Vector2.down * rayLength, _isGround ? Color.green : Color.red);
    }
}
