using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private const float SPEED = 3; // 基本速度
    [SerializeField] private float SpeedForse = 1.5f; // 速度倍率
    [SerializeField] private PlayerCharacterData characterData; // 
    public InputInfo InputInfo {get; set;}
    // public InputInfo CharacterData {get; set;}

    private Rigidbody2D _rigidbody2d;
    private Weapon _weapon;
    void Awake()
    {
        _rigidbody2d = GetComponent<Rigidbody2D>();
        _weapon = GetComponentInChildren<Weapon>();
        if(characterData == null)
        {
            Debug.LogError("characterDataが設定されていません");
        }
    }

    void Update()
    {
        MovePlayer(InputInfo);
        WeaponAttackPlayer(InputInfo);
    }

    // プレイヤーの移動
    public void MovePlayer(InputInfo inputInfo)
    {
        _rigidbody2d.velocity = inputInfo.Move * characterData.MoveSpeed;
    }

    public void WeaponAttackPlayer(InputInfo inputInfo)
    {
        _weapon.IsTrigger = inputInfo.Attack;
    }
    
    private void SkillPlayer(InputInfo inputInfo)
    {

    }

    private void SpcialPlayer(InputInfo inputInfo)
    {

    }
}
