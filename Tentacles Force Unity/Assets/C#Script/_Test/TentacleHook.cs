using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// プレイヤーをこのオブジェクトへのベクトルの逆方向に移動させる
/// </summary>
public class TentacleHook : MonoBehaviour
{
    [SerializeField] private GameObject Player; // プレイヤー
    [SerializeField] private Rigidbody2D playerRb; // プレイヤーのRigidbody2D
    [SerializeField] private float minHorizontalForce = 2f; // X軸の移動を保証するための最小値
    [SerializeField] private float jumpForceMultiplier = 10f; // 距離を力に変換する係数

    private TentaclesJump _tentaclesJump;
    private bool isGround = false;
    void Start()
    {
        if(Player == null)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
        }
        _tentaclesJump = Player.GetComponent<TentaclesJump>();
        playerRb = Player.GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 地面に衝突した場合
        if (collision.gameObject.CompareTag("Ground") && isGround == false) 
        {
            // プレイヤーの位置とオブジェクトHの位置のベクトルを取得
            Vector2 hookPosition = transform.position;
            Vector2 playerPosition = playerRb.position;
            Vector2 directionToHook = hookPosition - playerPosition;

            // 反転してジャンプ方向を決定
            Vector2 jumpDirection = -directionToHook.normalized;

            // X軸の影響を強くするため、X成分が小さすぎる場合は補正
            if (Mathf.Abs(jumpDirection.x) < 0.2f)
            {
                jumpDirection.x = Mathf.Sign(jumpDirection.x) * minHorizontalForce / jumpForceMultiplier;
                jumpDirection.Normalize();
            }
            // 距離を基準にジャンプ力を計算
            float distance = directionToHook.magnitude;
            Vector2 jumpForce = jumpDirection * distance * jumpForceMultiplier;

            // プレイヤーに力を加える
            // playerRb.velocity = Vector2.zero; // 現在の速度をリセット
            // playerRb.AddForce(jumpForce, ForceMode2D.Impulse);

            // プレイヤーにジャンプを適用
            _tentaclesJump.ApplyTentacleJump(jumpForce);

            Debug.Log($"Jump Force Applied: {jumpForce}");
            isGround = true;
        }
    }
}
