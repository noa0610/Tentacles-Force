using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator Animator { get; private set; }

    // クリップ名と長さのキャッシュ
    private Dictionary<string, float> _clipLengthDict = new Dictionary<string, float>();

    // 現在のnormalizedTimeを保持
    private float _currentNormalizedTime = 0f;

    void Awake()
    {
        Animator = GetComponent<Animator>();

        // AnimatorController内の全クリップをキャッシュ
        if (Animator != null && Animator.runtimeAnimatorController != null)
        {
            foreach (var clip in Animator.runtimeAnimatorController.animationClips)
            {
                if (!_clipLengthDict.ContainsKey(clip.name))
                {
                    _clipLengthDict.Add(clip.name, clip.length);
                    Debug.Log($"Cached clip: {clip.name}, Length: {clip.length}");
                }
            }
        }
    }

    /// <summary>
    /// 現在再生中のアニメーションクリップ名を取得
    /// </summary>
    public string GetCurrentClipName()
    {
        var clips = Animator.GetCurrentAnimatorClipInfo(0);
        if (clips.Length > 0)
        {
            return clips[0].clip.name;
        }
        return string.Empty;
    }

    /// <summary>
    /// 指定したクリップ名の長さを取得（キャッシュから）
    /// </summary>
    public float GetClipLength(string clipName)
    {
        if (_clipLengthDict.TryGetValue(clipName, out float length))
        {
            return length;
        }
        return 0f;
    }

    /// <summary>
    /// 現在再生中のクリップの長さを取得
    /// </summary>
    public float GetCurrentClipLength()
    {
        string name = GetCurrentClipName();
        return GetClipLength(name);
    }

    /// <summary>
    /// 現在再生中のアニメーションのnormalizedTimeを取得し、フィールドに保存
    /// </summary>
    public void UpdateCurrentNormalizedTime()
    {
        if (Animator == null) {
            _currentNormalizedTime = 0f;
            return;
        }
        var stateInfo = Animator.GetCurrentAnimatorStateInfo(0);
        _currentNormalizedTime = stateInfo.normalizedTime % 1f;
    }

    /// <summary>
    /// 保持しているnormalizedTimeを取得
    /// </summary>
    public float GetSavedNormalizedTime()
    {
        return _currentNormalizedTime;
    }

    /// <summary>
    /// 現在のクリップと異なる場合のみ指定したクリップを再生する。
    /// </summary>
    /// <param name="clipName"></param>
    /// <param name="normalizedTime"></param>
    public void PlayIfChanged(string clipName, float normalizedTime = 0f)
    {
        string current = GetCurrentClipName();
        if (current != clipName)
        {
            Animator.Play(clipName, 0, normalizedTime);
        }
    }
}
