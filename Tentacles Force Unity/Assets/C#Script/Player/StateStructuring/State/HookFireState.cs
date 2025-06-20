using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookFireState : IPlayerState
{
    public void Enter(Player2 player)
    {
        if (player.Hook.currentHookState == HookSystem.HookState.Fire)
        {
            return;
        }
        else if (player.Hook.currentHookState == HookSystem.HookState.Idle)
        {
            player.Hook.ResetPosision(player.Object.transform.position);
        }

        // クリップが "Move"なら"Walk_Fire"に切り替える
        string currentClip = player.Anime.GetCurrentClipName();
        if (currentClip == "Walk")
        {
            player.Anime.PlayIfChanged("Walk_Fire", player.Anime.GetSavedNormalizedTime());
            player.Anime.Animator.SetBool("Move", false);
            player.Anime.Animator.SetBool("Idle_Fire", false);
            player.Anime.Animator.SetBool("Move_Fire", false);
        }

        // フックを発射
        player.Hook.StartFire();
    }

    public void Execute(Player2 player, InputInformation input)
    {
        player.Move.GetVelocity(); // 現在の速度を取得

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

        // 向きの更新
        player.Move.Direction(input.Move);
        // ------------------------------------------------

        // ================================================ジャンプ
        if (input.Jump && !player.Move.AirJudge())
        {
            player.Anime.Animator.SetBool("Idle_Fire", false);
            player.Move.Jump(); // ジャンプ処理を実行
            player.StateTransition(this, new AirHookFireState());
        }
        // ------------------------------------------------

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
            player.Anime.Animator.SetBool("Idle_Fire", false);
            player.Hook.StartRewind(); // フックを巻き戻す
            player.StateTransition(this, new GroundMoveState());
        }
        // -----------------------------------------------


        // ================================================フック命中
        // 地面に命中したらHit状態に遷移
        if (player.Hook.currentHookState == HookSystem.HookState.Hit)
        {
            player.StateTransition(this, new HookHitState());
        }
        // 敵に命中したらEnemyHit状態に遷移
        else if (player.Hook.currentHookState == HookSystem.HookState.EnemyHit)
        {
            player.StateTransition(this, new HookAttackState());
        }
        // -----------------------------------------------

        // ================================================空中移動の移行
        if (player.Move.AirJudge())
        {
            player.Anime.Animator.SetBool("Idle_Fire", false);
            player.Anime.Animator.SetBool("Move_Fire", false);
            player.StateTransition(this, new AirHookFireState());
        }
        // ------------------------------------------------

        player.Move.SetVelocity(); // 計算した速度をセット
    }

    public void Exit(Player2 player)
    {
        // クリップが "Walk_Fire" の場合、現在の正規化された時間を更新
        string currentClip = player.Anime.GetCurrentClipName();
        if (currentClip == "Walk_Fire")
        {
            player.Anime.UpdateCurrentNormalizedTime();
        }
    }
}
