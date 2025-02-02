using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundLoop : MonoBehaviour
{
    private const float k_maxLength = 1f; // 背景のループ処理の範囲を1で設定
    private const string k_propName = "_MainTex"; // シェーダープロパティ名を定義（MainTexのオフセットを変更するため）

    [SerializeField] private Vector2 m_offsetSpeed = Vector2.one; // オフセットの移動速度を設定する変数。Inspectorで設定可能
    [SerializeField] private GameObject Player; // プレイヤーのTransformを参照するための変数

    private Material m_material; // 背景に適用するマテリアル
    private Vector3 previousPlayerPosition; // 前フレームのプレイヤーの位置を保持するための変数

    private void Start()
    {
        if (GetComponent<SpriteRenderer>() is SpriteRenderer sr) // スプライトレンダラーからマテリアルを取得
        {
            m_material = sr.material;
        }

        if (Player == null)
        {
            Player = GameObject.FindWithTag("Player");
        }
        previousPlayerPosition = Player.transform.position; // プレイヤーの初期位置を設定
    }

    private void Update()
    {
        if (m_material && Player)
        {
            // プレイヤーの移動量を計算
            Vector3 playerDelta = Player.transform.position - previousPlayerPosition;
            previousPlayerPosition = Player.transform.position;

            // プレイヤーの移動に基づいて背景のオフセットを計算
            float x = Mathf.Repeat(m_material.GetTextureOffset(k_propName).x + (playerDelta.x * m_offsetSpeed.x), k_maxLength);
            float y = Mathf.Repeat(m_material.GetTextureOffset(k_propName).y + (playerDelta.y * m_offsetSpeed.y), k_maxLength);
            var offset = new Vector2(x, y);

            // 計算したオフセットをマテリアルに適用
            m_material.SetTextureOffset(k_propName, offset);
        }
    }

    private void OnDestroy()
    {
        // ゲームをやめた後にマテリアルのOffsetを戻しておく
        if (m_material)
        {
            m_material.SetTextureOffset(k_propName, Vector2.zero);
        }
    }
}