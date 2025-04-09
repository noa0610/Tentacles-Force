using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayBGM : MonoBehaviour
{
    // インスペクターから指定する変数

    // 内部処理する変数
    private AudioSource _audioSource;
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.Play();
    }

}
