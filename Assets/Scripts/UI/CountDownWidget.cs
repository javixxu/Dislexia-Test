using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class CountdownWidget : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;

    private Coroutine countdownCoroutine;


    private void Start()
    {
        //tofdo hacer invisiblesi nose usa
        gameObject.SetActive(false);
    }

    public void StartCountdown(int seconds, Action onCountdownFinished)
    {
        gameObject.SetActive(true);

        // Si ya hay un countdown en ejecución, lo cancelamos
        if (countdownCoroutine != null)
            StopCoroutine(countdownCoroutine);

        countdownCoroutine = StartCoroutine(CountdownRoutine(seconds, onCountdownFinished));
    }

    private IEnumerator CountdownRoutine(int seconds, Action onCountdownFinished)
    {
        gameObject.SetActive(true);
        int counter = seconds;

        while (counter > 0)
        {
            if (countdownText != null)
                countdownText.text = counter.ToString();

            yield return new WaitForSeconds(1f);
            counter--;
        }

        if (countdownText != null)
            countdownText.text = "¡GO!";

        onCountdownFinished?.Invoke();

        gameObject.SetActive(false);
    }
}