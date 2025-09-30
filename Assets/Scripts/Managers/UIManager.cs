using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    GameManager gameManager;
    public Canvas mainCanvas;

    [SerializeField]
    Transform ExercisePanel;

    [SerializeField]
    CountdownWidget countdownWidget;

    [SerializeField]
    TimerWidget timerWidget;


    [SerializeField]
    ScoreWidget ScoreWidget;


    [SerializeField]
    InfoWidget InfoWidget;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple UIManager instances detected! Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Debug.Log("Initializing UI MANAGER");
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.timeSubsystem.OnTimeOver += HandleTimeOver;
    }

    private void HandleTimeOver(){
        
    }

    public Canvas GetMainCanvas() { return mainCanvas; }

    public void ShowCountdown(int seconds, Action onCountdownFinished)
    {
        if (countdownWidget != null)
        {
            if (!countdownWidget.gameObject.activeSelf)
                countdownWidget.gameObject.SetActive(true);

            countdownWidget.StartCountdown(seconds, onCountdownFinished);
        }
        else
        {
            Debug.LogWarning("CountdownWidget is not assigned in UIManager.");
            onCountdownFinished?.Invoke();
        }
    }
    public Transform GetExercisePanel() { return ExercisePanel; }

    public ScoreWidget GetScoreWidget() { return ScoreWidget; }
};
