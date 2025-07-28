using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class EnemyDefeatedEffectPlayer : MonoBehaviour
{
    [SerializeField] private GameObject effectPrefab;

    private void Start()
    {
        MessageBroker.Default
            .Receive<EnemyDefeatedMessage>()
            .Subscribe(msg =>
            {
                Instantiate(effectPrefab, msg.Position, Quaternion.identity);
            })
            .AddTo(this);
    }
}
