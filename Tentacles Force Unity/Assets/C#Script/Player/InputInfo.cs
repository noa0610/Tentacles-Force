using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 入力の情報を総括して管理するクラス
/// </summary>
public class InputInfo
{
    public Vector2 Move {get; set;}
    public bool Jump {get; set;}
    public bool Attack {get; set;}
    public bool Skill {get; set;}
    public bool Spcial {get; set;}
    public bool Change {get; set;}
    public bool Pose {get; set;}
}
