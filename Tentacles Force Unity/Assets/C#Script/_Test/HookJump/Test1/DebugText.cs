using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugText : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    public TextMeshProUGUI _textMeshProUGUI;
    private HookJump _hookJump;

    
    public TextMeshProUGUI _InputTextMeshProUGUI;
    private bool isPushA;
    private bool isPushD;

    void Start()
    {
        if(_textMeshProUGUI == null)
        {
            _textMeshProUGUI = this.gameObject.GetComponent<TextMeshProUGUI>();
        }

        if(Player == null)
        {
            // プレイヤーのオブジェクトを探す
            Player = GameObject.FindWithTag("Player");
        }

        // プレイヤーのスクリプトを取得
        _hookJump = Player.GetComponent<HookJump>();
    }

    void Update()
    {
        isPushA = false;
        isPushD = false;

        if(Input.GetKey(KeyCode.A))
        {
            isPushA = true;
        }
        else if(Input.GetKey(KeyCode.D))
        {
            isPushD = true;
        }

        _textMeshProUGUI.text = $"isHookFired = {_hookJump.IsHookFired} \n isStartHookFired = {_hookJump.IsStartHookFired} \n isHookRewind = {_hookJump.IsHookRewind} \n isHookHit = {_hookJump.IsHookHit}";
        _InputTextMeshProUGUI.text = $"Left = {isPushA} \nRight = {isPushD}";
    }
}
