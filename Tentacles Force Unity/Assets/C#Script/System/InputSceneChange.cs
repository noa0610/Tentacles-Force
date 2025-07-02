using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キーボードのキーで指定したシーンに移動するスクリプト
/// </summary>
public class InputSceneChange : MonoBehaviour
{
    // インスペクターから設定する変数
    [SerializeField] private string SceneChangeName;  // 移動するシーン名
    [SerializeField] private string SE_Name;          // SEの名前
    [SerializeField] private float ChangeTime = 0.0f; // シーン遷移までの時間（秒）
    [SerializeField] private KeyCode keyCode = KeyCode.Return; // シーン遷移を行うキー
    [SerializeField] private int MouseNumber = 0;

    void Update()
    {
        if (Input.GetKeyDown(keyCode) || Input.GetMouseButtonDown(MouseNumber))
        {
            if (SE_Name != null)
            {
                SoundManager.Instance.PlaySE(SE_Name); // SEを再生
            }
            SceneChangeManager.Instance.ChangeSceneLoad(SceneChangeName, ChangeTime);
        }
    }
}
