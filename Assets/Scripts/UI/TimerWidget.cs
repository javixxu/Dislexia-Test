
using TMPro;
using UnityEngine;

class TimerWidget: MonoBehaviour
{
    private TextMeshProUGUI timerText;
    private GameManager gameManager;

    private void Awake()
    {
        timerText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;

        // Suscribirse a los eventos del TimeSubsystem
        gameManager.timeSubsystem.OnSecondPassed += UpdateUI;

        // Inicializar la UI con el tiempo actual
        UpdateUI((int)gameManager.timeSubsystem.GetCurrentTime());
    }

    private void UpdateUI(int secondsRemaining)
    {
        timerText.text = $"Tiempo: {secondsRemaining}s";
    }

}

