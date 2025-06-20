using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private AttackData attackData; // 攻撃データ
    public AttackData AttackData
    {
        get { return attackData; } // 攻撃データを取得
        set { attackData = value; } // 攻撃データを設定
    }
    private bool _isHit = false; // ヒットフラグ
    private bool _isTargetHit = false; // ターゲットヒットフラグ
    public bool IsTargetHit
    {
        get { return _isTargetHit; }
    }

    private Collider2D _collider2D; // コライダー
    private List<int> _hitIDList = new List<int>(); // ヒットしたオブジェクトのIDリスト

    private void Start()
    {
        _collider2D = GetComponent<Collider2D>();

        _collider2D.enabled = false; // コライダーを無効化
    }

    public void Init()
    {
        _isHit = false;              // ヒットフラグをリセット
        _isTargetHit = false;        // ターゲットヒットフラグをリセット
        _collider2D.enabled = false; // コライダーを無効化
        IDListClear();
    }

    public void HitOn()
    {
        _collider2D.enabled = true; // コライダーを有効化
        _isHit = true;              // ヒットフラグをセット
        _isTargetHit = false;       // ターゲットヒットフラグをリセット
    }
    public void HitOff()
    {
        _collider2D.enabled = false; // コライダーを無効化
        _isHit = false;              // ヒットフラグをリセット
        _isTargetHit = false;        // ターゲットヒットフラグをリセット
    }
    
    public void IDListClear()
    {
        _hitIDList.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isHit) return; // ヒットフラグが立っていない場合は処理を終了
        if (_hitIDList.Contains(other.GetInstanceID())) return; // 既にヒットしたオブジェクトの場合は処理を終了

        var unit = other.GetComponent<UnitBase>();
        if (unit == null) return;

        // 攻撃対象が設定されている場合
        if (attackData.target != null)
        {
            if (other.gameObject == attackData.target)
            {
                unit.TakeDamage(attackData.power);
                _hitIDList.Add(other.GetInstanceID());
                _isTargetHit = true;
            }
            return;
        }

        // 攻撃者のタグとユニットのタグが異なる場合のみダメージを与える
        if ((attackData.attackerTag == UnitTags.Player && unit.UnitStatus.unitTags == UnitTags.Enemy) ||
            (attackData.attackerTag == UnitTags.Enemy && unit.UnitStatus.unitTags == UnitTags.Player))
        {
            unit.TakeDamage(attackData.power);

            _hitIDList.Add(other.GetInstanceID()); // ヒットしたオブジェクトのIDをリストに追加
        }
    }

    // 攻撃範囲を設定するメソッド
    // public void SetAttackRange(float range)
    // {
    //     if (_collider2D is CircleCollider2D circleCollider)
    //     {
    //         circleCollider.radius = range; // CircleCollider2Dの場合、半径を設定
    //     }
    //     else if (_collider2D is BoxCollider2D boxCollider)
    //     {
    //         boxCollider.size = new Vector2(range * 2, boxCollider.size.y); // BoxCollider2Dの場合、幅を設定
    //     }
    //     // 他のコライダータイプに応じて処理を追加することも可能
    // }
}