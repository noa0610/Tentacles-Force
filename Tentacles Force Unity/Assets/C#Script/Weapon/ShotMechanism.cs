using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShotMechanism : ScriptableObject, IWeaponShotMechanism
{
    public abstract void Fire(GameObject shotPoint, GameObject prefab, float speed, Transform target = null);
}
