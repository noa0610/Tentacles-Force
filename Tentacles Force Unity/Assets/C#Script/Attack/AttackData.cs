using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackData
{
    public int power; // 攻撃力
    public float range; // 攻撃範囲
    public float duration; // 攻撃の持続時間
    public float knockbackPower; // ノックバックの力
    public UnitTags attackerTag; // 攻撃者のタグ（プレイヤー、敵など）
    public GameObject target; // 攻撃対象のゲームオブジェクト(nullの場合は全ての敵に攻撃)
}
