using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirHookFireState : IPlayerState
{
    float time = 0; // 時間計測用変数
    public void Enter(Player2 player)
    {
        player.Anime.Animator.SetBool("Fall_Fire", true);
        player.Anime.Animator.SetBool("Move_Fire", false);
        player.Anime.Animator.SetBool("Move", false);
        if (player.Hook.currentHookState == HookSystem.HookState.Fire)
        {
            return;
        }
        else if (player.Hook.currentHookState == HookSystem.HookState.Idle)
        {
            player.Hook.ResetPosision(player.Object.transform.position);
        }

        // フックを発射
        player.Hook.StartFire();
    }

    public void Execute(Player2 player, InputInformation input)
    {
        time += Time.deltaTime;
        player.Move.GetVelocity(); // 現在の速度を取得

        // ================================================空中移動
        // 地面移動の入力を更新
        player.Move.GroundMove(input.Move);

        // 向きの更新
        player.Move.Direction(input.Move);
        // ------------------------------------------------


        // ================================================地面接地
        if (!player.Move.AirJudge() && time > 0.1f)
        {
            player.Anime.Animator.SetBool("Fall_Fire", false);
            player.StateTransition(this, new HookFireState());
        }
        // -------------------------------------------------

        // ================================================フック移動
        // マウス左入力されている状態なら
        if (input.MouseLeft)
        {
            player.Hook.GetRopeRayPoint(); // フックのレイを飛ばす

            // フックをマウス追従させる
            player.Hook.MousePursueUpdate();
        }
        // マウス左入力がなかったら
        else if (!input.MouseLeft)
        {
            player.Hook.StartRewind(); // フックを巻き戻す
            player.Anime.Animator.SetBool("Fall_Fire", false);
            player.StateTransition(this, new AirMoveState());
        }
        // -----------------------------------------------

        // フックが命中したらフック命中状態に遷移
        if (player.Hook.currentHookState == HookSystem.HookState.Hit)
        {
            player.StateTransition(this, new AirHookHitState());
        }

        player.Move.SetVelocity(); // 計算した速度をセット
    }

    public void Exit(Player2 player)
    {

    }
}
