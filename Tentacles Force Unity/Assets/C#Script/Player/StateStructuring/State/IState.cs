/// <summary>
/// ステートのインターフェース
/// </summary>
public interface IState<T>
{
    /// <summary>
    /// そのステートになった瞬間の処理
    /// </summary>
    /// <param name="owner"></param>
    void Enter(T owner);

    /// <summary>
    /// そのステート中、毎フレーム行われる処理
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="inputInformation"></param>
    void Execute(T owner, InputInformation inputInformation);

    /// <summary>
    /// 次のステートに切り替わる瞬間の処理
    /// </summary>
    /// <param name="owner"></param>
    void Exit(T owner);
}
