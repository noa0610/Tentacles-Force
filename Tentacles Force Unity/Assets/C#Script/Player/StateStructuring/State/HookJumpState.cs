using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookJumpState : IPlayerState
{
    float _exitTime = 0;
    float _maxExitTime = 0.5f; // ジャンプの持続時間

    public void Enter(Player2 player)
    {
        // フックまでの距離とベクトル依存の力でジャンプ
        player.Move.ActionJump(player.Hook.GetHookDirection(), player.Hook.GetHookDistance());

        // フックの方向に向きを更新
        player.Move.Direction(player.Hook.GetHookDirection());

        SoundManager.Instance.PlaySE("jump09_HookJump"); // ジャンプ音を再生
        player.Anime.Animator.SetBool("HookJump", true);
    }

    public void Execute(Player2 player, InputInformation input)
    {
        _exitTime += Time.deltaTime;

        // 現在の速度を取得
        player.Move.GetVelocity();

        // ================================================空中移動
        // 地面移動の入力を更新
        player.Move.GroundMove(input.Move);

        // 向きの更新
        player.Move.Direction(input.Move);
        // ------------------------------------------------

        // ================================================地面接地
        if (_exitTime >= _maxExitTime)
        {
            if (player.Move.AirJudge())
            {
                player.StateTransition(this, new AirMoveState());
            }
            else if (!player.Move.AirJudge())
            {
                player.StateTransition(this, new GroundMoveState());
            }
        }
        // -------------------------------------------------


        // 計算した速度をセット
        player.Move.SetVelocity();
    }

    public void Exit(Player2 player)
    {
        player.Anime.Animator.SetBool("HookJump", false);
    }
}