using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;


[System.Serializable]
public class ExerciseEntry
{
    public int typeId;
    public GameObject prefab;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    //Exercises
    public ExercisesSystem exercisesSystem { get; private set; }
    public List<ExerciseEntry> TypeOfExercisesList;
    Dictionary<int, GameObject> TypeOfExercises; //TODO: A more flexible structure should be adopted. 
    public ExerciseBase currentExercise { get; private set; }

    //Time
    public TimeSubsystem timeSubsystem { get; private set; }

    public struct AnswersCounter
    {
        public int success;
        public int errors;
    }
    AnswersCounter answersCounter = new AnswersCounter();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // evita duplicados
            return;
        }

        Debug.Log("Initializasing GAME MANAGER");
        Instance = this;

        string path = Path.Combine(Application.streamingAssetsPath, "data_exercises.json");

        exercisesSystem = new ExercisesSystem(path);

         TypeOfExercises = new Dictionary<int, GameObject>();
        foreach (var entry in TypeOfExercisesList)
        {
            if (!TypeOfExercises.ContainsKey(entry.typeId))
                TypeOfExercises.Add(entry.typeId, entry.prefab);
        }

        timeSubsystem = new TimeSubsystem();

        DontDestroyOnLoad(gameObject); // no se destruye al cambiar de escena
    }

    private void Start()
    {
        exercisesSystem.OnPackageChanged += HandlePackageChanged;

        OnExerciseFinished(); // Get a new Exercise
    }

    void HandlePackageChanged(Package newPackage)
    {
        timeSubsystem.SetTime(newPackage.time);

        currentExercise?.gameObject?.SetActive(false);
        timeSubsystem.Pause();
        UIManager.Instance.ShowCountdown(5, () =>
        {
            // Esto se ejecuta cuando la cuenta atrás termina
            Debug.Log("¡Comenzamos el paquete " + newPackage.typeId + "!");
            timeSubsystem.Resume();
            currentExercise?.gameObject.SetActive(true);
        });
    }

    public void OnExerciseFinished()
    {
        DestroyLastExercise();

        //Get a random diferentExercise
        if (exercisesSystem == null)
        {
            Debug.LogError("ExercisesSystem no está inicializado.");
            return;
        }

        Exercise nextExercise = exercisesSystem.GetNextExercise();
        if (nextExercise == null)
        {
            Debug.Log("No quedan más ejercicios en ningún paquete.");
            return;
        }

        Debug.Log("Siguiente ejercicio: " + nextExercise.id);

        int typeId = exercisesSystem.GetCurrentPackageTypeId();
        if (TypeOfExercises.ContainsKey(typeId))
        {
            GameObject gmO = Instantiate(TypeOfExercises[typeId], UIManager.Instance.GetMainCanvas().transform);
            currentExercise = gmO.GetComponent<ExerciseBase>();
            if (!currentExercise)
            {
                Debug.LogError("Exersise generated is not an exercise");
                return;
            }

            currentExercise?.Show(nextExercise);
            currentExercise.onExerciseFinished += OnExerciseFinished;
        }
        else
        {
            Debug.LogWarning($"No hay prefab asignado para el typeId {typeId}");
        }
    }

    void DestroyLastExercise()
    {
        if (currentExercise?.gameObject!=null)
            Destroy(currentExercise.gameObject);
    }

    private void Update()
    {
        timeSubsystem?.Update(Time.deltaTime);

    }

    public void UpdateAnswersCounter(bool bRight)
    {
        if (bRight)
        {
            answersCounter.success++;
        }
        else
        {
            answersCounter.errors++;
        }

        Debug.Log($"Aciertos: {answersCounter.success} - Errores: {answersCounter.errors}");
    }
}

