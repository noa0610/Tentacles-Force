using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UIにフェードインとフェードアウトを実行できる静的クラス
/// </summary>
public class UIFade : MonoBehaviour
{
    /// <summary>
    /// フェードインを行うコルーチン
    /// </summary>
    /// <param name="graphic"></param>
    /// <param name="duration"></param>
    /// <param name="startAlpha"></param>
    /// <param name="endAlpha"></param>
    /// <returns></returns>
    public static IEnumerator FadeIn(Graphic graphic, float duration, float startAlpha = 0f, float endAlpha = 1f)
    {
        if (graphic == null) yield break;

        graphic.canvasRenderer.SetAlpha(startAlpha); // 初期アルファ値を設定
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            graphic.canvasRenderer.SetAlpha(alpha); // アルファ値を更新
            yield return null;
        }

        graphic.canvasRenderer.SetAlpha(endAlpha); // 最終アルファ値を設定
    }

    /// <summary>
    /// フェードアウトを行うコルーチン
    /// </summary>
    /// <param name="graphic"></param>
    /// <param name="duration"></param>
    /// <param name="startAlpha"></param>
    /// <param name="endAlpha"></param>
    /// <returns></returns>
    public static IEnumerator FadeOut(Graphic graphic, float duration, float startAlpha = 1f, float endAlpha = 0f)
    {
        if (graphic == null) yield break;

        graphic.canvasRenderer.SetAlpha(startAlpha); // 初期アルファ値を設定
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            graphic.canvasRenderer.SetAlpha(alpha); // アルファ値を更新
            yield return null;
        }

        graphic.canvasRenderer.SetAlpha(endAlpha); // 最終アルファ値を設定
    }
}
