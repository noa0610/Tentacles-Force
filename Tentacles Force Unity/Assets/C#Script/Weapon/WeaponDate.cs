using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "WeaponData/WeaponData")]
public class WeaponDate : ScriptableObject
{
    [Header("武器の名前")]
    public string WeaponName;           // 武器の名前

    [Header("武器性能")]
    public GameObject AttackPrefab;     // 発射する攻撃プレハブ
    public float FireRate;              // 攻撃間隔 (秒)
    public float AttackSpeed;           // 攻撃の移動速度

    [Header("発射機構")]
    public IWeaponShotMechanism AttackLogic; // 発射ロジック
    
    [Header("効果")]
    public AudioClip FireSE;            // 発射音
    public ParticleSystem MuzzleEffect; // 発射エフェクト

    [Header("見た目")]
    public Sprite WeaponImage;          // 武器の画像
}
