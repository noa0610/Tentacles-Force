using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRate60 : MonoBehaviour
{
    /// <summary>
    /// フレームレートを60で固定する
    /// </summary>
    void Start()
    {
        Application.targetFrameRate = 60;
    }
}
