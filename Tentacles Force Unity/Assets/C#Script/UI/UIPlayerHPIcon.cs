using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* プレイヤーのHPを表示するUIクラス
 * 
 * 
 */
public class UIPlayerHelese : MonoBehaviour
{
    [SerializeField] private UnitBase PlayerStatus; // プレイヤーステータス
    [SerializeField] private GameObject HPIconPrefab;     // HPアイコン
    [SerializeField] private GameObject HPLostIconPrefab; // 失ったHPアイコン

    private List<GameObject> hpIcons = new List<GameObject>();
    private int beforeHP;  // 前回のHPを記録

    void Start()
    {
        CreateHPIcon();
        beforeHP = PlayerStatus.CurrentHealth;
        UpdateHPIcon();
    }

    void Update()
    {
        if (beforeHP != PlayerStatus.CurrentHealth)
        {
            UpdateHPIcon();
            beforeHP = PlayerStatus.CurrentHealth;
        }
    }

    private void CreateHPIcon()
    {
        // 既存アイコンを削除
        foreach (var icon in hpIcons)
        {
            Destroy(icon);
        }
        hpIcons.Clear();

        int maxHP = PlayerStatus.UnitStatus.maxHealth;
        for (int i = 0; i < maxHP; i++)
        {
            GameObject icon = Instantiate(HPIconPrefab, transform);
            hpIcons.Add(icon);
        }
    }

    private void UpdateHPIcon()
    {
        int currentHP = PlayerStatus.CurrentHealth;

        for (int i = 0; i < hpIcons.Count; i++)
        {
            Image img = hpIcons[i].GetComponent<Image>();
            if (img == null) continue;

            if (i < currentHP)
            {
                // 残HP分は通常アイコン
                img.sprite = HPIconPrefab.GetComponent<Image>().sprite;
                img.color = Color.white;
            }
            else
            {
                // 失ったHP分はロストアイコン
                if (HPLostIconPrefab != null)
                {
                    img.sprite = HPLostIconPrefab.GetComponent<Image>().sprite;
                    img.color = Color.white;
                }
                else
                {
                    // ロストアイコンが無ければ半透明に
                    img.color = new Color(1f, 1f, 1f, 0.3f);
                }
            }
        }
    }
}
