/// <summary>
/// プレイヤーのステートのインターフェース
/// </summary>
public interface IPlayerState
{
    /// <summary>
    /// そのステートになった瞬間の処理
    /// </summary>
    /// <param name="player"></param>
    void Enter(Player2 player);

    /// <summary>
    /// そのステート中、毎フレーム行われる処理
    /// </summary>
    /// <param name="player"></param>
    /// <param name="inputInformation"></param>
    void Execute(Player2 player, InputInformation inputInformation);

    /// <summary>
    /// 次のステートに切り替わる瞬間の処理
    /// </summary>
    /// <param name="player"></param>
    void Exit(Player2 player);
}
