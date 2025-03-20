using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookSystem : MonoBehaviour
{
    [SerializeField] private GameObject hookPrefab;
    [SerializeField] private float hookLengthMax = 10f;
    [SerializeField] private float retractSpeed = 5f;
    [SerializeField] private float rewindSpeed = 5f;
    [SerializeField] private float jumpSpeedmultiplier = 2f;
    [SerializeField] private LayerMask groundLayer;

    private GameObject currentHook;
    private Vector3 currentMousePos;
    private Vector3 hookTargetPosition;
    private float initialDistance;
    private bool isFired;
    private bool isRewind;
    private bool isStartHookFired;
    private bool isHit;
    private Rigidbody2D _rb2DHook;

    public bool IsHit => isHit;
    public bool IsFired => isFired;
    public bool IsRewind => isRewind;
    public Vector3 HookTargetPosition { get; set; }

    private void Awake()
    {
        // 初期化処理
        if (hookPrefab != null)
        {
            currentHook = hookPrefab;
            currentHook.SetActive(false);
        }
    }

    public void FireHook()
    {
        // フック発射処理
    }

    public void HookRewind()
    {
        // フック巻き戻し処理
    }

    public void HookPowerJump(Vector2 direction)
    {
        // フックジャンプ処理
    }

    public Vector2 GetHookDirection()
    {
        // フックの方向を取得
        return (hookTargetPosition - transform.position).normalized;
    }
}