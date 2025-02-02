using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class Weapon : MonoBehaviour
{
    [SerializeField] WeaponDate weaponData; // 武器データ
    [SerializeField] GameObject ShotPoint; // 発射位置
    public bool IsTrigger { get; set; } // 発射トリガー

    private float lastFireTime; // 最後に発射した時刻
    private SpriteRenderer _SpriteRenderer; // 画像変更
    private GameObject effect; // 発射位置に追従する発射エフェクト
    private AudioSource _audioSource; // 効果音
    void Start()
    {
        // スプライトの変更
        if (weaponData.WeaponImage != null)
        {
            _SpriteRenderer = GetComponent<SpriteRenderer>();
            _SpriteRenderer.sprite = weaponData.WeaponImage;
        }

        // 最初の発射ができるよう調整
        lastFireTime = -weaponData.FireRate;

        // 発射位置の設定
        if (ShotPoint == null)
        {
            ShotPoint = this.gameObject;
        }

        // 効果音の設定
        if (weaponData.FireSE != null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        // トリガーが押されて発射間隔の時間が空いている時
        if (IsTrigger && CanFire())
        {
            FireWeapon(); // 攻撃プレハブを発射

            // 音を再生
            if (weaponData.FireSE != null)
            {
                _audioSource.PlayOneShot(weaponData.FireSE);
            }
        }

        // エフェクトの位置を発射位置に揃える
        if (effect != null)
        {

            effect.transform.position = ShotPoint.transform.position;
        }


    }

    // 現在の時間と最後に発射した時間で発射間隔の時間が空いているか判定
    private bool CanFire()
    {
        return Time.time - lastFireTime >= weaponData.FireRate;
    }

    private void FireWeapon()
    {
        // 発射ロジックの実行
        weaponData.ShotLogic.Fire(ShotPoint,
                                    weaponData.AttackPrefab,
                                    weaponData.AttackSpeed);

        // 発射時刻を更新
        lastFireTime = Time.time;

        // 発射エフェクトの生成
        if (weaponData.MuzzleEffect != null)
        {
            // エフェクトプレハブを生成
            GameObject effect = Instantiate(weaponData.MuzzleEffect.gameObject,
                                            ShotPoint.transform.position,
                                            weaponData.MuzzleEffect.transform.rotation);
            Destroy(effect, 0.5f); // エフェクトプレハブを一定時間後に削除
        }
    }
}
