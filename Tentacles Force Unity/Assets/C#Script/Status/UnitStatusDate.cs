using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UnitStatus")]
public class UnitStatusDate : ScriptableObject
{
    public string unitName; // ユニット名
    public int maxHealth; // 最大HP
    public int attackPower; // 攻撃力
    public int defensePower; // 防御力
    public UnitTags unitTags; // ユニットのタグ（プレイヤー、敵など）
}