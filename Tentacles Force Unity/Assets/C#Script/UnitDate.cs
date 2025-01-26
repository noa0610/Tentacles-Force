using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitDate
{
    public enum Team
    {
        Neutral, // 中立
        Player,  // プレイヤー
        Enemy    // 敵
    }

    // ※ 中立は初期化のみに使用され、ゲームプレイに使用されません (Neutral is used only for initialization and is not utilized in gameplay.)
}