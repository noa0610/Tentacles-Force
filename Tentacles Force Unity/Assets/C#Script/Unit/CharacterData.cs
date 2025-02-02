using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseCharacterData : ScriptableObject
{
    [Header("キャラクター名")]
    public string CharacterName;         // キャラクター名

    [Header("ステータス")]
    public int MaxHP;                    // 最大HP
    public float MoveSpeed;              // 移動速度
    public float JumpForce;              // ジャンプ力
    public UnitDate.Team UnitTeam;       // 敵か味方か

    [Header("演出効果")]
    public AudioClip DestroySE;          // 撃破SE
    public ParticleSystem DestroyEffect; // 撃破エフェクト

    [Header("見た目")]
    public Sprite CharacterImage;        // キャラクターの画像
}
