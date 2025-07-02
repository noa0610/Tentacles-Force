using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * シーン遷移を管理するシングルトン
 *
 * シーン遷移を非同期で行うためのクラスです。
 * シングルトンパターンを使用して、他のスクリプトから簡単にアクセスできるようにしています。
 * 
 * 使用方法:
 * 1. シーン遷移を行いたいスクリプトから、SceneChangeManager.Instance.ChangeSceneLoad("SceneName")を呼び出します。
 * 2. 遷移するシーン名は引数で指定します。
 * 
 * また、引数で指定した時間後にシーン遷移を行うオーバーロードメソッドも用意しています。
 */

public class SceneChangeManager : MonoBehaviour
{
    // シングルトンのインスタンスを保持するプロパティ
    public static SceneChangeManager Instance { get; private set; }

    // 内部処理する変数
    private bool isChangingScene = false; // シーン遷移中かを判定するフラグ
    private void Awake()
    {
        // シングルトンパターンの実装
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンが変わってもこのオブジェクトを破棄しない
        }
        else
        {
            Destroy(gameObject); // 既にインスタンスが存在する場合は、このオブジェクトを破棄
        }
    }

    // 指定されたシーンに遷移するメソッド
    public void ChangeSceneLoad(string sceneName)
    {
        if (isChangingScene) return;

        isChangingScene = true; // 遷移中のフラグを設定
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    // シーン遷移を遅延させるコルーチンを呼び出すメソッド(オーバーロード)
    public void ChangeSceneLoad(string sceneName, float changetime)
    {
        if (isChangingScene) return;
        
        StartCoroutine(ChangeSceneWithDelay(sceneName, changetime));
    }

    // 非同期でシーンをロード
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.completed += _ => isChangingScene = false; // ロード完了時にフラグを解除
        yield return asyncLoad; // ロード完了を待機
    }

    // 実際のコルーチン処理は内部で管理
    private IEnumerator ChangeSceneWithDelay(string sceneName, float delay)
    {
        isChangingScene = true; // 遷移中のフラグを設定
        yield return new WaitForSeconds(delay); // 指定の時間待機
        yield return LoadSceneAsync(sceneName); // シーンロードを非同期で処理
    }
}