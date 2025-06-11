using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地上移動のステート
/// </summary>
public class GroundMoveState : IPlayerState
{
    public void Enter(Player2 player)
    {
        // クリップが "Walk_Fire"なら"Walk"に切り替える
        string currentClip = player.Anime.GetCurrentClipName();
        if (currentClip == "Walk_Fire")
        {
            player.Anime.PlayIfChanged("Walk", player.Anime.GetSavedNormalizedTime());
            player.Anime.Animator.SetBool("Move", true);
            player.Anime.Animator.SetBool("Move_Fire", false);
        }
    }

    public void Execute(Player2 player, InputInformation input)
    {
        player.Move.GetVelocity(); // 現在の速度を取得

        // ================================================地面移動の入力
        if (input.Move == Vector2.zero)
        {
            player.Move.GroundMoveBrake();
            player.Anime.Animator.SetBool("Move", false);
        }
        else
        {
            player.Move.GroundMove(input.Move);
            player.Anime.Animator.SetBool("Move", true);
        }

        // 向きの更新
        player.Move.Direction(input.Move);

        // ------------------------------------------------


        // ================================================ジャンプ入力
        if (input.Jump && !player.Move.AirJudge())
        {
            player.Anime.Animator.SetBool("Move", false);
            player.StateTransition(this, new JumpState());
        }
        // ------------------------------------------------

        // ================================================フック移動入力
        if (input.MouseLeft)
        {
            // フック発射状態に遷移
            player.StateTransition(this, new HookFireState());
        }
        // -----------------------------------------------

        // ================================================空中移動の移行
        if (player.Move.AirJudge())
        {
            player.Anime.Animator.SetBool("Move", false);
            player.StateTransition(this, new AirMoveState());
        }
        // ------------------------------------------------

        player.Move.SetVelocity(); // 計算した速度をセット
    }

    public void Exit(Player2 player)
    {
        // クリップが "Walk" の場合、現在の正規化された時間を更新
        string currentClip = player.Anime.GetCurrentClipName();
        if (currentClip == "Walk" || currentClip == "Walk_Fire")
        {
            player.Anime.UpdateCurrentNormalizedTime();
        }
    }
}
