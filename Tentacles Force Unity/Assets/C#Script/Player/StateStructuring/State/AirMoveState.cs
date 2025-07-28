using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 空中の移動のステート
/// </summary>
public class AirMoveState : IState<Player2>
{
    public void Enter(Player2 player)
    {
        player.Anime.Animator.SetBool("Fall", true);
    }

    public void Execute(Player2 player, InputInformation input)
    {
        player.ChackDead();
        
        player.Move.GetVelocity(); // 現在の速度を取得

        // ================================================空中移動の入力
        // 入力が無い場合は慣性移動
        if (input.Move.x != 0)
        {
            // 地面移動の入力を更新
            player.Move.GroundMove(input.Move);

            // 向きの更新
            player.Move.Direction(input.Move);
        }
        // ------------------------------------------------

        // ================================================地面接地
        if (!player.Move.AirJudge())
        {
            // 地面に触れたら
            player.StateTransition(this, new GroundMoveState());
        }
        // -------------------------------------------------

        // ================================================フック移動入力
        if (input.MouseLeft)
        {
            // フック発射状態に遷移
            player.StateTransition(this, new AirHookFireState());
        }
        // -----------------------------------------------

        // ================================================無敵状態の点滅処理
        player.InvincibleFlashing();
        // -----------------------------------------------

        player.Move.SetVelocity(); // 計算した速度をセット
    }

    public void Exit(Player2 player)
    {
        player.Anime.Animator.SetBool("Fall", false);
    }
}
