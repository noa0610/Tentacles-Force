using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// InputSystemの入力情報を管理するクラス
/// </summary>
public class InputInformation
{
    public Vector2 Move {get; set;}
    public bool Jump {get; set;}
    public bool MouseLeft {get; set;}
    public bool MouseRight {get; set;}
    public bool Cancel {get; set;}
    public bool Pose {get; set;}
}
