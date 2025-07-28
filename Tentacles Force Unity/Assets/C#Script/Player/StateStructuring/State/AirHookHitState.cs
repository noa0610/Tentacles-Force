using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirHookHitState : IState<Player2>
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
        player.Anime.Animator.SetBool("Fall_Fire", true);
        SoundManager.Instance.PlaySE("胸ぐらをつかむ_HookHit");
    }

    public void Execute(Player2 player, InputInformation input)
    {
        player.ChackDead();
        
        // 現在の速度を取得
        player.Move.GetVelocity();

        // ================================================空中移動
        // 地面移動の入力を更新
        player.Move.GroundMove(input.Move);

        // ------------------------------------------------


        // ================================================地面接地
        if (!player.Move.AirJudge())
        {
            // 地面に触れたら
            player.Anime.Animator.SetBool("Fall_Fire", false);
            player.StateTransition(this, new HookHitState());
        }
        // -------------------------------------------------


        // マウス左入力されている状態
        if (input.MouseLeft)
        {
            player.Hook.HitUpdate();
        }
        // ================================================フックジャンプ
        // マウス左入力が外れた時
        else if (!input.MouseLeft)
        {
            player.Hook.StartRewind();
            player.Anime.Animator.SetBool("Fall_Fire", false);
            player.StateTransition(this, new HookJumpState());
        }
        
        if (player.Hook.IsHookHitLengthLimit())
        {
            player.Hook.StartRewind();
            player.Anime.Animator.SetBool("Fall_Fire", false);
            player.StateTransition(this, new HookJumpState());
        }
        // -----------------------------------------------


        // ================================================フック巻き戻し
        if (input.MouseRight) // マウス右を押すとフックの巻き戻し
        {
            player.Hook.StartRewind();
            player.Anime.Animator.SetBool("Fall_Fire", false);
            player.StateTransition(this, new AirMoveState());
        }
        // -----------------------------------------------

        // ================================================無敵状態の点滅処理
        player.InvincibleFlashing();
        // -----------------------------------------------

        // 計算した速度をセット
        player.Move.SetVelocity();
    }

    public void Exit(Player2 player)
    {
        if (player.Hook.currentHookState == HookSystem.HookState.Hit)
        {
            // すでにフックが命中している場合は何もしない
            return;
        }
    }
}