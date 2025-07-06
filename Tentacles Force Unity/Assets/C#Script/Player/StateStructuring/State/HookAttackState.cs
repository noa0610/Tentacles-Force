using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookAttackState : IPlayerState
{
    private GameObject enemy; // 敵のGameObjectを保持する変数
    private float _exitTime = 0f; // 攻撃の経過時間
    private float _masExitTime = 3f; // 攻撃の持続時間

    public void Enter(Player2 player)
    {
        player.ResetAlpha(); // プレイヤーの透明度をリセット
        
        enemy = player.Hook.GetEnemy(); // フックが命中した敵のGameObjectを取得

        // アニメーションの設定
        player.Anime.Animator.SetBool("HookAttack", true);
        player.Anime.Animator.SetBool("Move_Fire", false);
        player.Anime.Animator.SetBool("Idle_Fire", false);
        player.Anime.Animator.SetBool("Fall_Fire", false);

        player.Move.SetGravityScale(0); // 重力スケールを0
        player.Move.ResetVelocity(); // 速度をリセット
        player.Move.MoveTowardsTargetImpulse(enemy.transform); // 敵に向かって移動

        player.Attack.AttackData.target = enemy; // 攻撃対象を設定
        player.Attack.Init();  // 攻撃の初期化
        player.Attack.HitOn(); // 攻撃を有効にする

        player.Status.InvincibilityOn(); // 無敵状態にする
    }

    public void Execute(Player2 player, InputInformation input)
    {
        player.ChackDead();
        
        _exitTime += Time.deltaTime;
        
        player.Move.TowardsRotation(enemy.transform, 90); // +90で敵と垂直の向き
        player.Move.GetVelocity();
        player.Move.MoveTowardsTarget(enemy.transform); // 敵に向かって移動

        player.Hook.AttackUpdate();
        enemy = player.Hook.GetEnemy(); // 敵の位置を更新
        
        // 敵がいない、または攻撃時間が経過した場合の処理
        if (enemy == null || _exitTime >= _masExitTime)
        {
            // 敵がいない場合はフックを巻き戻す
            player.Hook.StartRewind();
            player.StateTransition(this, new AirMoveState());
        }

        // フックが命中した敵にヒットした場合
        if (player.Attack.IsTargetHit)
        {
            player.Hook.StartRewind();
            player.Move.BrakeVelocity(0.5f); // 移動速度を減速
            SoundManager.Instance.PlaySE("lasergun_PlayerAttackHit"); // 攻撃音を再生
            player.StateTransition(this, new JumpState());
        }

        player.Move.SetVelocity();
    }

    public void Exit(Player2 player)
    {
        player.Attack.HitOff(); // 攻撃を無効にする

        player.Move.ResetRotation(); // 回転をリセット
        player.Move.ResetGravityScale(); // 重力スケールをリセット
        player.Move.Direction(enemy.transform.position - player.transform.position); // 向きを敵の方向に設定

        player.Anime.Animator.SetBool("HookAttack", false);

        player.Status.InvincibilityOff(); // 無敵状態を解除
    }
}
