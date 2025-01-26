using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackDate
{
    public int AttackPower; // 攻撃力
    public int HitCount;    // 攻撃ヒット数
    public int Penetration; // 攻撃貫通力
    public UnitDate.Team OwnerTeam;  // 攻撃を生成したチーム
    public UnitDate.Team TargetTeam; // 攻撃対象のチーム

    // 引数なしコンストラクタ ※初期化用
    public AttackDate()
    {
        AttackPower = 1;
        HitCount = 1;
        Penetration = 1;
        OwnerTeam = UnitDate.Team.Neutral;
        TargetTeam = UnitDate.Team.Neutral;
    }

    // 引数ありコンストラクタ
    public AttackDate(int attackPower, int hitCount, int penetration, UnitDate.Team ownerTeam)
    {
        AttackPower = attackPower;
        HitCount = hitCount;
        Penetration = penetration;
        OwnerTeam = ownerTeam;
        TargetTeam = GetOppositeTeam(ownerTeam);
    }

    // 引数ありコンストラクタ　※初期化用
    public AttackDate(int attackPower, int hitCount, int penetration)
    {
        AttackPower = attackPower;
        HitCount = hitCount;
        Penetration = penetration;
        OwnerTeam = UnitDate.Team.Neutral;
        TargetTeam = UnitDate.Team.Neutral;
    }

    // 所有チームに応じて攻撃対象のチームを設定する
    public UnitDate.Team GetOppositeTeam(UnitDate.Team ownerteam)
    {
        return ownerteam == UnitDate.Team.Player ? UnitDate.Team.Enemy : UnitDate.Team.Player;
    }

    // 攻撃対象のチームを手動で切り替える
    public void SwitchTargetTeam()
    {
        TargetTeam = GetOppositeTeam(TargetTeam);
    }

    /*メモ：コンストラクタとはインスタンスを生成するときに最初に実行されるメソッドのこと*/
}
