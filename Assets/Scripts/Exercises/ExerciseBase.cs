using System;
using UnityEngine;

public abstract class ExerciseBase : MonoBehaviour
{

    public Action onExerciseFinished;

    // Show: receive parsed ExerciseData
    public abstract void Show(Exercise data);

    public abstract void CheckSolution(int index);

    protected void Finish()
    {
        onExerciseFinished?.Invoke();
    }
}