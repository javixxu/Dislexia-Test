using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    ExerciseBase CurrentExercise;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // evita duplicados
            return;
        }

        Debug.Log("Initializasing GAME MANAGER");
        Instance = this;
        DontDestroyOnLoad(gameObject); // no se destruye al cambiar de escena
    }

    void OnExerciseFinished()
    {
        //Get a random diferentExercise
    }
}
