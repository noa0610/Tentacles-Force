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
        // アニメーションを待機に変更
        // player.Anime.Animator.SetTrigger("Move");
    }

    public void Execute(Player2 player, InputInformation input)
    {
        // 現在の入力を更新
        player.Move.CurrentMoveInput = input.Move;

        // 移動入力がある場合
        if (input.Move.x != 0)
        {
            player.Move.GroundSideMove(input.Move);
            player.Move.MoveBrake(input.Move);
        }
        // 移動入力がなく、地面にいる場合
        else if(input.Move.x == 0 && player.Move.CheckGround() == true)
        {
            player.Move.GroundMoveStop();
        }
        
        // 向きの更新
        player.Move.Turnaround(input.Move);

        // ジャンプの入力があり、地面に触れていたら
        if (input.Jump && player.Move.CheckGround() == true)
        {
            // ジャンプ
            player.Move.GroundJump();
        }

        // マウス左入力があった時
        if (input.MouseLeft)
        {
            // フックをアクティブ化
        }
    }

    public void Exit(Player2 player)
    {

    }
}
