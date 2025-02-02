using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerCharacterData characterData; // キャラクターデータ(ScriptableObject)
    [SerializeField] private LayerMask GroundLayer; // 地面のレイヤー
    [SerializeField] GameObject GroundCheck;
    public InputInfo InputInfo { get; set; }

    private Rigidbody2D _rigidbody2d;
    private Weapon _weapon;
    private bool _isGround; 
    void Awake()
    {
        _rigidbody2d = GetComponent<Rigidbody2D>();

        // 子オブジェクトから武器を取得
        _weapon = GetComponentInChildren<Weapon>();
        if (characterData == null)
        {
            Debug.LogError("characterDataが設定されていません");
        }

        // キャラクター画像変更
        if (characterData.CharacterImage != null)
        {
            SpriteRenderer _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = characterData.CharacterImage;
        }
    }

    void Update()
    {
        // 左右移動
        SideMove(InputInfo);

        // ジャンプ
        GroundJump(InputInfo);

        // 武器攻撃
        WeaponAttackPlayer(InputInfo);
    }

    private void FixedUpdate()
    {
        // 地面判定取得
        CheckGround();
    }

    // プレイヤーの移動
    public void SideMove(InputInfo inputInfo)
    {
        _rigidbody2d.velocity = new Vector2(inputInfo.Move.x * characterData.MoveSpeed, _rigidbody2d.velocity.y);
        Direction(inputInfo.Move);
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

    // ジャンプ
    public void GroundJump(InputInfo inputInfo)
    {
        if(inputInfo.Jump && _isGround)
        {
            Debug.Log("Jump");
            _rigidbody2d.velocity = new Vector2(_rigidbody2d.velocity.x, characterData.JumpForce);
        }
    }

    // 武器攻撃
    public void WeaponAttackPlayer(InputInfo inputInfo)
    {
        _weapon.IsTrigger = inputInfo.Attack;
    }

    private void SkillPlayer(InputInfo inputInfo)
    {

    }

    private void SpcialPlayer(InputInfo inputInfo)
    {

    }

    // 地面判定取得
    private void CheckGround()
    {
        
        float rayLength = 0.1f;
        _isGround = Physics2D.Raycast(GroundCheck.transform.position, Vector2.down, rayLength, GroundLayer);

        Debug.Log($"ChackGround : {_isGround}");
        // レイを表示（緑で表示、判定取得で赤で表示）
        Debug.DrawRay(GroundCheck.transform.position, Vector2.down * rayLength ,_isGround ? Color.green : Color.red);
    }
}
