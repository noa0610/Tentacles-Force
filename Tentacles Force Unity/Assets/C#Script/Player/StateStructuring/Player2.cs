using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの情報をまとめるクラス
/// </summary>
public class Player2 : MonoBehaviour
{
    public StateManager<Player2> _stateManager;
    public HookSystem Hook { get; private set; }
    public UnitMovement Move { get; private set; }
    public PlayerAnimation Anime { get; private set; }
    public Attack Attack { get; private set; }
    public UnitBase Status { get; private set; }
    public GameObject Object { get; private set; }
    public SpriteRenderer SprRen { get; private set; }
    public Collider2D Coll { get; private set; }
    private float _invincibleFlashingTime = 0.15f; // 無敵状態の点滅間隔
    private float Timer = 0f; // 無敵状態の点滅タイマー
    void Awake()
    {
        Move = gameObject.AddComponent<UnitMovement>();
        Anime = gameObject.AddComponent<PlayerAnimation>();
        Attack = gameObject.GetComponentInChildren<Attack>();
        Status = gameObject.GetComponent<UnitBase>();

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

        _stateManager = new StateManager<Player2>();
        if (Hook != null)
        {
            _stateManager.Init(this, new GroundMoveState());
        }
        else
        {
            Debug.LogError("StateManager initialization skipped due to missing HookSystem.");
        }

        SprRen = GetComponent<SpriteRenderer>();
        Coll = GetComponent<Collider2D>();
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
    public void StateTransition(IState<Player2> fromState, IState<Player2> nextState)
    {
        _stateManager.Transition(this, fromState, nextState);
    }

    /// <summary>
    /// 無敵状態の点滅処理
    /// </summary>
    public void InvincibleFlashing()
    {
        if (Status.IsInvincible)
        {
            Timer += Time.deltaTime;
            if (Timer >= _invincibleFlashingTime)
            {
                // 透明度を切り替え
                Color color = SprRen.color;
                color.a = (color.a == 1f) ? 0.5f : 1f;
                SprRen.color = color;
                Timer = 0f;
            }
        }
        else
        {
            // 無敵解除時は透明度を1に戻す
            Color color = SprRen.color;
            if (color.a != 1f)
            {
                color.a = 1f;
                SprRen.color = color;
            }
            Timer = 0f;
        }
    }

    public void ResetAlpha()
    {
        if (SprRen != null)
        {
            Color color = SprRen.color;
            color.a = 1f;
            SprRen.color = color;
        }
    }

    public void ChackDead()
    {
        if (Status.IsDead)
        {
            StateTransition(_stateManager._CurrentState, new DeadState());
        }
    }
}
