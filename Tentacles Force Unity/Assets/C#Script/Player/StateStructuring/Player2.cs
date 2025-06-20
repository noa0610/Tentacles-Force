using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの情報をまとめるクラス
/// </summary>
public class Player2 : MonoBehaviour
{
    public PlayerStateManager _stateManager;
    public HookSystem Hook { get; private set; }
    public UnitMovement Move { get; private set; }
    public PlayerAnimation Anime { get; private set; }
    public Attack Attack { get; private set; }
    public GameObject Object { get; private set; }

    void Awake()
    {
        Move = gameObject.AddComponent<UnitMovement>();
        Anime = gameObject.AddComponent<PlayerAnimation>();
        Attack = gameObject.GetComponentInChildren<Attack>();
        if (Attack == null)
        {
            Debug.LogError("Attack component is not found in Player2 GameObject.");
        }
        Hook = gameObject.GetComponent<HookSystem>();

        Object = gameObject;
        if (Hook == null)
        {
            Debug.LogError("HookSystem is not attached to the Player2 GameObject.");
        }

        _stateManager = new PlayerStateManager();
        if (Hook != null)
        {
            _stateManager.Init(this);
        }
        else
        {
            Debug.LogError("StateManager initialization skipped due to missing HookSystem.");
        }
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
