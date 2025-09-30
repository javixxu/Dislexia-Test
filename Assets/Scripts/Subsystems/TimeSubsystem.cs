

using System;

public class TimeSubsystem
{
    bool bPaused;
    float TimeRemaining;

    int lastWholeSecond; // último segundo entero registrado

    public event Action OnTimeOver;
    public event Action<int> OnSecondPassed; // se pasa el segundo actual

    public TimeSubsystem()
    {
        TimeRemaining = 0;
        bPaused = true;
        lastWholeSecond = 0;
    }
    public TimeSubsystem(int totalTime)
    {
        TimeRemaining = totalTime;
        bPaused = true;
        lastWholeSecond = (int)Math.Floor(TimeRemaining);
    }

    public void Update(float deltaTime)
    {
        if (bPaused)
            return;

        TimeRemaining -= deltaTime;
        if (TimeRemaining < 0)
            TimeRemaining = 0;

        int currentWholeSecond = (int)Math.Floor(TimeRemaining);

        // Evento de paso de segundo
        if (currentWholeSecond < lastWholeSecond)
        {
            OnSecondPassed?.Invoke(currentWholeSecond);
        }
        lastWholeSecond = currentWholeSecond;

        // Evento de tiempo agotado
        if (TimeRemaining <= 0)
            OnTimeOver?.Invoke();
    }

    public float GetCurrentTime()
    {
        return TimeRemaining;
    }

    public void SetTime(float newTime)
    {
        TimeRemaining = newTime;
        lastWholeSecond = (int)Math.Floor(TimeRemaining);

        OnSecondPassed?.Invoke(lastWholeSecond);
    }
    public void Pause() => bPaused = true;
    public void Resume() => bPaused = false;
}