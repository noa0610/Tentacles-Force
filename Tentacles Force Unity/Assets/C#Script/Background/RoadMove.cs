using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoadMove : MonoBehaviour
{
	/// <summary>
	/// 道の画像をループさせる処理
	/// </summary>
	
	// インスペクターから指定
	[SerializeField] private Vector2 m_offsetSpeed;

	// 内部変数
	private const float k_maxLength = 1f; // 最大値の定数
	private const string k_propName = "_MainTex"; // マテリアルのオフセットを戻す定数
	private Material m_material; // マテリアル
	private void Start()
	{

		if (GetComponent<SpriteRenderer>() is SpriteRenderer i) // オブジェクトにSpriteRendererがあれば
		{
			m_material = i.material; // SpriteRendererに設定されたマテリアルを取得
		}
	}

	private void Update()
	{
		if (m_material)
		{
			// xとyの値が0 ～ 1でリピートするようにする
			var x = Mathf.Repeat(Time.time * m_offsetSpeed.x, k_maxLength);
			var y = Mathf.Repeat(Time.time * m_offsetSpeed.y, k_maxLength);
			var offset = new Vector2(x, y);
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