using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator Animator {get; private set;}

    void Awake()
    {
        Animator = GetComponent<Animator>();
    }
}
