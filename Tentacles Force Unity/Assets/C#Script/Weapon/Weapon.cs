using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class Weapon : MonoBehaviour
{
    [SerializeField] WeaponDate weaponData; // 武器データ
    [SerializeField] GameObject ShotPoint; // 発射位置

    public bool IsTrigger {get; set;}
    private SpriteRenderer _SpriteRenderer;
    private GameObject effect;
    void Start()
    {
        if (weaponData.WeaponImage != null)
        {
            // スプライトを変更
            _SpriteRenderer = GetComponent<SpriteRenderer>();
            _SpriteRenderer.sprite = weaponData.WeaponImage;
        }

        // 発射位置の設定
        if (ShotPoint == null)
        {
            ShotPoint = this.gameObject;
        }
    }

    void Update()
    {
        if (IsTrigger)
        {
            // 攻撃を発射
            FireBulletAttack(ShotPoint, 
                             weaponData.AttackPrefab, 
                             weaponData.AttackSpeed, 
                             weaponData.MuzzleEffect);
        }

        // エフェクトの位置を揃える
        if(effect != null)
        {
            effect.transform.position = ShotPoint.transform.position;
        }
    }

    // 弾を発射
    private void FireBulletAttack(GameObject shotPoint, GameObject prefab, float speed, ParticleSystem fireeffect)
    {
        // 攻撃プレハブを生成
        GameObject instance = Instantiate(prefab, 
                                          shotPoint.transform.position, 
                                          shotPoint.transform.rotation);

        // 発射方向設定
        float direction = transform.localScale.x > 0 ? 1f : -1f;

        // 攻撃プレハブのリジッドボディ取得
        Rigidbody2D rb = GetRigidbody2D(instance);

        // キネマティックをオン
        rb.isKinematic = true;

        // 弾のプレハブを移動させる
        rb.velocity = new Vector2(direction * speed, rb.velocity.y);

        // エフェクトのプレハブを生成
        effect = Instantiate(fireeffect.gameObject, 
                                            shotPoint.transform.position, 
                                            fireeffect.gameObject.transform.rotation);

        // エフェクトをすぐに削除
        Destroy(effect, 0.5f);
    }

    // オブジェクトのリジッドボディを取得
    private Rigidbody2D GetRigidbody2D(GameObject gameObject)
    {
        Rigidbody2D rb;
        if (gameObject.GetComponent<Rigidbody2D>())
        {
            Debug.Log("Rigidbody2D有りのプレハブ");
            rb = gameObject.GetComponent<Rigidbody2D>();
        }
        else
        {
            Debug.Log("Rigidbody2D無しのプレハブ");
            rb = gameObject.AddComponent<Rigidbody2D>(); // リジッドボディ追加
        }
        return rb;
    }
}
