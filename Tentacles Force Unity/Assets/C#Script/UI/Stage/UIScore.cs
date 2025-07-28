using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using UniRx;

public class UIScore : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private TextMeshProUGUI ScoreText;

    private void Start()
    {
        scoreManager.Score // スコアが変動するたびに自動でコールバック
            .Subscribe(value =>
            {
                ScoreText.text = $"SCORE: {value}";
            })
            .AddTo(this);
    }
}