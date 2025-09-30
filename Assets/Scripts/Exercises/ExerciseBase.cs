using System;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
///Abstract base class for any type of exercise in the game.
///Provides the minimum structure that specific exercises must implement, 
///as well as common functionalities such as ending the exercise.
/// </summary>
public abstract class ExerciseBase : MonoBehaviour
{
    // When Exercises is finished, this event is triggered.
    public Action onExerciseFinished;

    // Initialize and display the exercise with the provided data.
    public abstract void Show(Exercise data);

    // Check the player's solution.
    public abstract void CheckSolution(int index);

    //setup the UI elements specific to the exercise type
    protected abstract void SetupUI();

    protected void Finish()
    {
        onExerciseFinished?.Invoke();
    }

    protected System.Collections.IEnumerator EndAfter(float s)
    {
        yield return new WaitForSeconds(s);
        Finish();
    }
}