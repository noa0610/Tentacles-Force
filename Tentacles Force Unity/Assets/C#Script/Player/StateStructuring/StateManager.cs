using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステートの切り替え処理が定義されたクラス
/// </summary>
public class StateManager<T>
{
    public IState<T> _CurrentState { get; private set; }

    /// <summary>
    /// 現在のステートから次のステートへの切り替えを行う
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="fromState"></param>
    /// <param name="nextState"></param>
    public void Transition(T owner, IState<T> fromState, IState<T> nextState)
    {
        if (_CurrentState != fromState) return;

        _CurrentState.Exit(owner);
        _CurrentState = nextState;
        Debug.Log("StateChange : " + _CurrentState.ToString());
        _CurrentState.Enter(owner);
        
    }

    /// <summary>
    /// GroundMoveState状態に初期化するメソッド
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="initialState"></param>
    public void Init(T owner, IState<T> initialState)
    {
        _CurrentState = initialState;
        _CurrentState.Enter(owner);
    }
}
