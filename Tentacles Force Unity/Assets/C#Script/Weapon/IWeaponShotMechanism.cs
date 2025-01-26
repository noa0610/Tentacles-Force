using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 弾の発射動作のインターフェース
public interface IWeaponShotMechanism
{
    void Fire(GameObject shotPoint, GameObject prefab, float speed, Transform target = null);
}
