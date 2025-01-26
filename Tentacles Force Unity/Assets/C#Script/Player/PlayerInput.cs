using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 入力状況を渡すスクリプト
/// </summary>
public class PlayerInput: MonoBehaviour
{
    private InputInfo _inputInfo;
    private Player _player;

    private void Awake()
    {
        _inputInfo = new InputInfo();     // InputInfo取得
        _player = GetComponent<Player>(); // Playerコンポーネント取得

        _player.InputInfo = _inputInfo; // 入力の状況を渡す
    }

    private void Update()
    {
        _player.InputInfo = _inputInfo; // 入力の状況を渡す
        
        // Debug.Log($"Move{_inputInfo.Move}");
        // Debug.Log($"Attack:{_inputInfo.Attack}");
        // Debug.Log($"Skill:{_inputInfo.Skill}");
        // Debug.Log($"Spcial:{_inputInfo.Spcial}");
        // Debug.Log($"Change:{_inputInfo.Change}");
        // Debug.Log($"Pose:{_inputInfo.Pose}");

        InputClear(); // 入力のクリア
    }

    private void OnMove(InputValue value) // 移動入力受け取り
    {
        var input = value.Get<Vector2>();
        _inputInfo.Move = new Vector3(input.x, input.y, 0);
    }

    private void OnAttack() // 攻撃入力受け取り
    {
        _inputInfo.Attack = true;
    }

    private void OnSkill() // スキル入力受け取り
    {
        _inputInfo.Skill = true;
    }

    private void OnSpcial() // 必殺技入力受け取り
    {
        _inputInfo.Spcial = true;
    }

    private void OnChange() // キャラクター入れ替え入力受け取り
    {
        _inputInfo.Change = true;
    }

    private void OnPose() // ポーズ入力受け取り
    {
        _inputInfo.Pose = true;
    }

    private void InputClear()
    {
        _inputInfo.Skill = false;
        _inputInfo.Attack = false;
        _inputInfo.Spcial = false;
        _inputInfo.Change = false;
        _inputInfo.Pose = false;
    }
}