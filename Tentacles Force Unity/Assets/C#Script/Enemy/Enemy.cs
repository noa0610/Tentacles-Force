using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int EnemyHP = 30;

    private UnitDate _unitDate;
    public UnitDate.Team team = UnitDate.Team.Enemy;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        EnemyHP -= damage;
        Debug.Log($"EnemyHP:  {EnemyHP + damage} - {damage} = {EnemyHP}");

        if(EnemyHP <= 0)
        {
            UnitDestroy();
        }
    }

    public void UnitDestroy()
    {
        Destroy(gameObject);
    }
}
