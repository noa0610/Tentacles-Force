using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// InputInfomationに入力状況を渡すクラス
/// </summary>
public class PlayerInputDetection : MonoBehaviour
{
    private InputInformation _inputInformation;
    private Player2 _player;

    void Awake()
    {
        _inputInformation = new InputInformation();   // InputInformationをインスタンス化
        _player = GetComponent<Player2>();            // Player2コンポーネント取得

        
    }

    void Update()
    {
        // 入力の状況を渡す
        _player.StateExecute(_inputInformation);

        // 一瞬のみ判定を取得するボタンの判定をクリア
        InputClear();
    }

    // 移動の入力
    private void OnMove(InputValue value)
    {
        var input = value.Get<Vector2>();
        _inputInformation.Move = new Vector2(input.x, 0.0f);
    }

    // ジャンプの入力
    private void OnJump()
    {
        _inputInformation.Jump = true;
    }

    // マウス左の入力
    private void OnMouseLeft(InputValue value)
    {
        _inputInformation.MouseLeft = value.isPressed;
    }

    // マウス右の入力
    private void OnMouseRight(InputValue value)
    {
        _inputInformation.MouseRight = value.isPressed;
    }

    // 入力状況のクリア
    private void InputClear()
    {
        _inputInformation.Jump = false;
    }
}
