using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipSyncChild : MonoBehaviour
{
    [SerializeField] private SpriteRenderer targetRenderer; // 親キャラの SpriteRenderer
    private Vector3 defaultLocalPosition;

    void Start()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponentInParent<SpriteRenderer>();
        }

        defaultLocalPosition = transform.localPosition;
    }

    void LateUpdate()
    {
        if (targetRenderer == null) return;

        // flipX が true なら x を反転、それ以外は元に戻す
        float x = targetRenderer.flipX ? -defaultLocalPosition.x : defaultLocalPosition.x;
        transform.localPosition = new Vector3(x, defaultLocalPosition.y, defaultLocalPosition.z);
    }
}
