using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("AttackParameter")]
    [SerializeField] private int AttackPower; // 攻撃力
    [SerializeField] private UnitTags OwnerTeam; // 所属チーム

    private AttackDate attackDate;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {
            
        }
    }
}
