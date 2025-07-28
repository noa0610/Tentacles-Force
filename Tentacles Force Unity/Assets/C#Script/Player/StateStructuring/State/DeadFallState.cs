using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadFallState : IState<Player2>
{
    private float _exitTime = 0;
    private float _maxExitTime = 2f; // 持続時間
    private float _rotateSpeed = 360f; // 1秒で1回転

    public void Enter(Player2 player)
    {
        player.Move.Jump();
        player.Move.SetGravityScale(1.5f);

        player.Coll.enabled = false;
        _rotateSpeed *= (player.transform.localScale.x >= 0) ? 1f : -1f;
        SoundManager.Instance.PlaySE("death08_GameOver");
    }

    public void Execute(Player2 player, InputInformation input)
    {
        _exitTime += Time.deltaTime;

        // 回転しながら落下
        player.transform.Rotate(0, 0, _rotateSpeed * Time.deltaTime);

        // 時間が経過した場合の処理
        if (_exitTime >= _maxExitTime)
        {
            player.Move.ResetVelocity();
            player.Move.SetGravityScale(0);
            player.Move.ResetRotation();

            GameManager.Instance.CurrentGameState = GameState.GameOver;
        }
    }

    public void Exit(Player2 player)
    {

    }
}
