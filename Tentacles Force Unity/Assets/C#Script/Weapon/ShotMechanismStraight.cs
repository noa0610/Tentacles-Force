using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// IWeaponShotMechanism継承：一直線に発射する動作
/// </summary>

[CreateAssetMenu(fileName = "StraightShotMechanism", menuName = "WeaponLogic/Straight")]
public class ShotMechanismStraight : ShotMechanism
{
    public override void Fire(GameObject shotPoint, GameObject prefab, float speed, Transform target = null)
    {
        GameObject instance = Object.Instantiate(prefab, shotPoint.transform.position, shotPoint.transform.rotation);
        Rigidbody2D rb = instance.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(speed * shotPoint.transform.localScale.x, 0); // X方向に飛ばす
    }
}
