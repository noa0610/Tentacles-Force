using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookObject : MonoBehaviour
{
    private HookJump hookJump;

    void Start()
    {
        hookJump = GameObject.FindWithTag("Player").GetComponent<HookJump>();
    }
    public void SetHookJump(HookJump hook)
    {
        hookJump = hook;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground")) // 地面に触れたら
        {
            Debug.Log("HookHit");
            // hookJump.OnHookHit(transform.position);
            Destroy(gameObject); // フックを削除
        }
    }
}