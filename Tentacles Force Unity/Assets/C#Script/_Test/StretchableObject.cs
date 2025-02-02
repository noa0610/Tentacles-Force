using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// オブジェクトを自然に伸ばすスクリプト
/// </summary>
public class StretchableObject : MonoBehaviour
{
    private GameObject Player;
    private float maxStretchLength; // 伸びる長さの上限
    private float stretchSpeed; // 伸びる速度
    private float retractSpeed; // 縮む速度
    private float holdTime; // 伸びた後の待機時間
    private bool isStretching = true;
    private float currentLength = 0;
    private Vector3 basePosition;   // 基点の位置
    private Vector3 direction; // 伸びる方向

    public void Initialize(float maxLength, float stretchSpd, float retractSpd, float hold)
    {
        maxStretchLength = maxLength;
        stretchSpeed = stretchSpd;
        retractSpeed = retractSpd;
        holdTime = hold;
        
        StartCoroutine(StretchAndRetract());
    }

    void Start()
    {
        Player = GameObject.FindWithTag("Player");

        if (Player != null)
        {
            // 初期の基点位置をプレイヤー基準で設定
            basePosition = transform.position;
        }

        // 伸びる方向を保存
        direction = transform.right;
    }

    void Update()
    {
        if (Player != null)
        {
            // プレイヤーの移動に応じて基点を更新
            basePosition = Player.transform.position;
        }

        if (isStretching)
        {
            if (currentLength < maxStretchLength)
            {
                float deltaLength = stretchSpeed * Time.deltaTime;
                currentLength += deltaLength;

                // 位置を基点から更新
                transform.position = basePosition + direction * (currentLength * 0.5f);

                // X軸方向にのみスケール変更
                transform.localScale = new Vector3(currentLength, transform.localScale.y, transform.localScale.z);
            }
            else
            {
                isStretching = false;
                StartCoroutine(Retract());
            }
        }
    }

    // オブジェクト伸縮処理
    private IEnumerator StretchAndRetract()
    {
        float currentLength = 0;

        while (currentLength < maxStretchLength)
        {
            float deltaLength = stretchSpeed * Time.deltaTime;
            currentLength += deltaLength;

            // 位置を更新（基点を維持しながら伸ばす）
            transform.position = basePosition + direction * (currentLength * 0.5f);

            // スケール更新（X軸のみ伸ばす）
            transform.localScale = new Vector3(currentLength, transform.localScale.y, transform.localScale.z);

            yield return null;
        }

        // 最大まで伸びたら一定時間待機
        yield return new WaitForSeconds(holdTime);

        // 縮む処理
        while (currentLength > 0)
        {
            float deltaLength = retractSpeed * Time.deltaTime;
            currentLength -= deltaLength;

            // 位置を更新（基点を維持しながら縮める）
            transform.position = basePosition + direction * (currentLength * 0.5f);

            // スケール更新
            transform.localScale = new Vector3(currentLength, transform.localScale.y, transform.localScale.z);

            yield return null;
        }

        // 縮み終わったら削除
        Destroy(gameObject);
    }

    private IEnumerator Retract()
    {
        yield return new WaitForSeconds(holdTime);
        while (currentLength > 0)
        {
            float deltaLength = retractSpeed * Time.deltaTime;
            currentLength -= deltaLength;

            // 位置を更新（基点を維持しながら縮める）
            transform.position = basePosition + direction * (currentLength * 0.5f);

            // スケール更新
            transform.localScale = new Vector3(currentLength, transform.localScale.y, transform.localScale.z);

            yield return null;
        }
        Destroy(gameObject);
    }
}
