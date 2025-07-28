using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : IState<Player2>
{
    float _exitTime = 0;
    float _maxExitTime = 0.5f; // ジャンプの持続時間
    public void Enter(Player2 player)
    {
        player.Move.Jump(); // ジャンプ処理を実行
        SoundManager.Instance.PlaySE("jump06_Playerjump"); // ジャンプ音を再生
        player.Anime.Animator.SetTrigger("Jump"); // ジャンプアニメーションを再生
        player.Anime.Animator.SetBool("OnJump", true); // 継続ジャンプフラグを立てる
    }

    public void Execute(Player2 player, InputInformation input)
    {
        player.ChackDead();
        
        _exitTime += Time.deltaTime;

        // 現在の速度を取得
        player.Move.GetVelocity();

        // ================================================空中移動
        // 地面移動の入力を更新
        player.Move.GroundMove(input.Move);

        // 向きの更新
        player.Move.Direction(input.Move);
        // ------------------------------------------------

        // ================================================フック移動入力
        if (input.MouseLeft)
        {
            // フック発射状態に遷移
            player.StateTransition(this, new AirHookFireState());
        }
        // -----------------------------------------------

        // ジャンプの持続時間が経過したら
        if (_exitTime >= _maxExitTime)
        {
            // 空中移動のステートに遷移
            player.StateTransition(this, new AirMoveState());
        }

        // ================================================無敵状態の点滅処理
        player.InvincibleFlashing();
        // -----------------------------------------------

        // 計算した速度をセット
        player.Move.SetVelocity();
    }

    public void Exit(Player2 player)
    {
        player.Anime.Animator.SetBool("OnJump", false); // 継続ジャンプフラグを下げる
    }
}