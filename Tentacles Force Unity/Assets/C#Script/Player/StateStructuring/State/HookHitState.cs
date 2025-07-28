using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookHitState : IState<Player2>
{
    public void Enter(Player2 player)
    {
        if (player.Hook.currentHookState == HookSystem.HookState.Hit)
        {
            // すでにフックが命中している場合は何もしない
            return;
        }
        // フックを命中検知した位置で固定
        player.Hook.HitPosisionStay();
        Debug.Log("SE");
    }

    public void Execute(Player2 player, InputInformation input)
    {
        player.ChackDead();
        
        // 現在の速度を取得
        player.Move.GetVelocity();

        // ================================================地面移動
        if (input.Move == Vector2.zero)
        {
            player.Move.GroundMoveBrake();
            player.Anime.Animator.SetBool("Idle_Fire", true);
            player.Anime.Animator.SetBool("Move_Fire", false);
        }
        else
        {
            player.Move.GroundMove(input.Move);
            player.Anime.Animator.SetBool("Move_Fire", true);
            player.Anime.Animator.SetBool("Idle_Fire", false);
        }
        // ------------------------------------------------


        // ================================================ハイジャンプ(フックが命中した位置より真上方向にジャンプ)
        // if (input.Jump && !player.Move.AirJudge())
        // {
        //     player.Move.Jump(); // ジャンプ
        //     player.StateTransition(this, new AirHookHitState());
        // }
        // ------------------------------------------------

        // マウス左入力されている状態
        if (input.MouseLeft)
        {
            player.Hook.HitUpdate();
        }
        // ================================================フックジャンプ
        // マウス左入力が外れたか、フックの長さが限界に達したら
        else if (!input.MouseLeft)
        {
            player.Hook.StartRewind();
            player.Anime.Animator.SetBool("HookJump", true);
            player.Anime.Animator.SetBool("Move_Fire", false);
            player.Anime.Animator.SetBool("Idle_Fire", false);
            player.StateTransition(this, new HookJumpState());
        }

        if (player.Hook.IsHookHitLengthLimit())
        {
            player.Hook.StartRewind();
            player.Anime.Animator.SetBool("HookJump", true);
            player.Anime.Animator.SetBool("Move_Fire", false);
            player.Anime.Animator.SetBool("Idle_Fire", false);
            player.StateTransition(this, new HookJumpState());
        }
        // -----------------------------------------------

            // ================================================フック巻き戻し
            // マウス右を押すとフックの巻き戻し
            if (input.MouseRight)
            {
                player.Hook.StartRewind(); // フックを巻き戻す
                player.StateTransition(this, new GroundMoveState());
            }
        // -----------------------------------------------

        // ================================================空中移動の移行
        if (player.Move.AirJudge())
        {
            player.Anime.Animator.SetBool("Move_Fire", false);
            player.StateTransition(this, new AirHookHitState());
        }
        // ------------------------------------------------

        // ================================================無敵状態の点滅処理
        player.InvincibleFlashing();
        // -----------------------------------------------

        // 計算した速度をセット
        player.Move.SetVelocity();
    }

    public void Exit(Player2 player)
    {
        
    }
}