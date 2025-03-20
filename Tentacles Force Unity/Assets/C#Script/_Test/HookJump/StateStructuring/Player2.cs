using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの情報をまとめるクラス
/// </summary>
public class Player2 : MonoBehaviour
{
    public PlayerStateManager _stateManager;

    public PlayerMovement Move { get; private set; }
    public PlayerAnimation Anime { get; private set; }

    void Awake()
    {
        Move = gameObject.AddComponent<PlayerMovement>();
        Anime = gameObject.AddComponent<PlayerAnimation>();

        _stateManager = new PlayerStateManager();
        _stateManager.Init(this);
    }

    void FixedUpdate()
    {
        
    }

    /// <summary>
    ///  現在のステートのExecuteの処理を継続する
    /// </summary>
    /// <param name="inputInformation"></param>
    public void StateExecute(InputInformation inputInformation)
    {
        _stateManager._CurrentState.Execute(this, inputInformation);
    }

    /// <summary>
    /// 現在のステートから次のステートへ切り替える
    /// </summary>
    /// <param name="fromState"></param>
    /// <param name="nextState"></param>
    public void StateTransition(IPlayerState fromState, IPlayerState nextState)
    {
        _stateManager.Transition(this, fromState, nextState);
    }
}
