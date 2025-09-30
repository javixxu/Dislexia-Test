using TMPro;
using UnityEngine;

public class ScoreWidget : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI correctText;
    [SerializeField] private TextMeshProUGUI wrongText;

    private void Start()
    {
        GameManager.Instance.onAnswersCounterChanged += UpdateUI;
    }

    private void UpdateUI(AnswersCounter counter)
    {
        correctText.text = $"{counter.success}";
        wrongText.text = $"{counter.errors}";
    }
}