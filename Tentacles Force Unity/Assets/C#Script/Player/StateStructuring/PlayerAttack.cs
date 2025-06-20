using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private AttackData attackData; // 攻撃データ
    public AttackData AttackData
    {
        set { attackData = value; } // 攻撃データを設定
    }
    void Start()
    {
        // GameObject instance = Instantiate(Resources.Load("Prefabs/Attack")) as GameObject;
    }

    public void Init()
    {
        // 初期化処理が必要な場合はここに記述
    }
}
