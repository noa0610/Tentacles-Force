using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("AttackParameter")]
    [SerializeField] private int AttackPower; // 攻撃力
    [SerializeField] private int HitCount;    // 攻撃ヒット数
    [SerializeField] private int Penetration; // 貫通力
    [SerializeField] public UnitDate.Team OwnerTeam; // 攻撃を生成したチーム

    private AttackDate attackDate;
    void Start()
    {
        attackDate = new AttackDate(AttackPower, HitCount, Penetration, OwnerTeam);
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            enemy.TakeDamage(attackDate.AttackPower);

            Penetration --;

            if(Penetration == 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
