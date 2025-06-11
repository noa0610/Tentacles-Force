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
    public PlayerMovement Move { get; private set; }
    public PlayerAnimation Anime { get; private set; }
    public GameObject Object { get; private set; }

    void Awake()
    {
        Move = gameObject.AddComponent<PlayerMovement>();
        Anime = gameObject.AddComponent<PlayerAnimation>();
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

    public T GetBehaviour<T>() where T : StateMachineBehaviour
    {
        return Anime.Animator.GetBehaviour<T>();
    }
}
