using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : IState<Player2>
{
    float _exitTime = 0;
    float _maxExitTime = 1f; // 持続時間

    public void Enter(Player2 player)
    {
        player.ResetAlpha(); // プレイヤーの透明度をリセット

        player.Anime.Animator.SetTrigger("Dead");

        player.Coll.enabled = false; // コライダーをオフ

        player.Move.ResetVelocity();
        player.Move.SetGravityScale(0);
        player.Move.ResetRotation();
        GameManager.Instance.CurrentGameState = GameState.None;
        SoundManager.Instance.PlaySE("damaged3_PlayerDead");
    }

    public void Execute(Player2 player, InputInformation input)
    {
        _exitTime += Time.deltaTime;

        // 時間が経過した場合の処理
        if (_exitTime >= _maxExitTime)
        {
            player.StateTransition(this, new DeadFallState());
        }
    }

    public void Exit(Player2 player)
    {

    }
}