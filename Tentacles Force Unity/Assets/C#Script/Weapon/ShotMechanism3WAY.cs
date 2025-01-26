using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotMechanism3WAY : IWeaponShotMechanism
{
    private float spreadAngle = 30f; // 発射角度の間隔
    private int bulletCount = 3;     // 発射する弾の数 
    public void Fire(GameObject shotPoint, GameObject prefab, float speed, Transform target = null)
    {
        for (int i = 0; i < bulletCount; i++)
        {
            // 弾の角度を計算
            float angle = -spreadAngle / 2 + spreadAngle / (bulletCount - 1) * i;

            // 回転を計算
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            // プレハブを生成
            GameObject instance = Object.Instantiate(prefab,
                                                     shotPoint.transform.position, 
                                                     rotation * shotPoint.transform.rotation);

            // 弾の移動方向を計算
            Vector2 direction = rotation * Vector2.right;

            // Rigidbody2Dを使って弾を移動させる
            Rigidbody2D rb = instance.GetComponent<Rigidbody2D>();
            rb.velocity = direction * speed;
        }
    }
}
