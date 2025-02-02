using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "CharacterData/PlayerData")]
public class PlayerCharacterData : BaseCharacterData
{
    [Header("固有アクション")]
    public AudioClip SpecialMoveSE;    // 特殊技の効果音
}
