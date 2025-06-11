using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステートの切り替え処理が定義されたクラス
/// </summary>
public class PlayerStateManager
{
    public IPlayerState _CurrentState { get; private set; }

    /// <summary>
    /// 現在のステートから次のステートへの切り替えを行う
    /// </summary>
    /// <param name="player"></param>
    /// <param name="fromState"></param>
    /// <param name="nextState"></param>
    public void Transition(Player2 player, IPlayerState fromState, IPlayerState nextState)
    {
        if (_CurrentState != fromState) return;

        _CurrentState.Exit(player);
        _CurrentState = nextState;
        Debug.Log("StateChange : " + _CurrentState.ToString());
        _CurrentState.Enter(player);
        
    }

    /// <summary>
    /// GroundMoveState状態に初期化するメソッド
    /// </summary>
    /// <param name="player"></param>
    public void Init(Player2 player)
    {
        _CurrentState = new GroundMoveState();
        _CurrentState.Enter(player);
        // player.Anime.Animator.ResetTrigger("MOVE");
    }
}
