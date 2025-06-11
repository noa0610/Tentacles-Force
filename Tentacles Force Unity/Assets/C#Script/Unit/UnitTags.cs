using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum UnitTags
{
    Neutral = 0,     // 無し
    Player = 1 << 0, // プレイヤー
    Enemy = 1 << 1   // 敵
}