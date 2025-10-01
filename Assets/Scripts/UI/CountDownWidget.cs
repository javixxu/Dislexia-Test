using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class CountdownWidget : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;

    private Coroutine countdownCoroutine;

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

            yield return new WaitForSeconds(1f); // Wait 1 sec
            counter--;
        }

        if (countdownText != null)
            countdownText.text = "¡GO!";

        yield return new WaitForSeconds(1f);

        onCountdownFinished?.Invoke();

        gameObject.SetActive(false);
    }
}