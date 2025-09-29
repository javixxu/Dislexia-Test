using System;
using UnityEngine;

public abstract class ExerciseBase : MonoBehaviour
{
    public Action onExerciseFinished;
    // Show: receive parsed ExerciseData
    public abstract void Show(object data);
    public abstract void ForceStop();

    protected void Finish()
    {
        onExerciseFinished?.Invoke();
    }
}