using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


[System.Serializable]
public class ExerciseEntry
{
    public int typeId;
    public GameObject prefab;
    public string soundInstruction;
}

[System.Serializable]
public struct AnswersCounter
{
    public int success;
    public int errors;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    //Exercises
    public ExercisesSystem exercisesSystem { get; private set; }
    public List<ExerciseEntry> TypeOfExercisesList;
    Dictionary<int, GameObject> TypeOfExercises;
    public ExerciseBase currentExercise { get; private set; }

    //Time
    public TimeSubsystem timeSubsystem { get; private set; }

    //Scores
    private AnswersCounter answersCounter;
    public Action<AnswersCounter> onAnswersCounterChanged;

    //Cant Move Until Load the next Exercise
    public bool bCanTakeObject { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // evita duplicados
            return;
        }

        Debug.Log("Initializasing GAME MANAGER");
        Instance = this;

        StartCoroutine(InitExerciseSystem());

         TypeOfExercises = new Dictionary<int, GameObject>();
        foreach (var entry in TypeOfExercisesList)
        {
            if (!TypeOfExercises.ContainsKey(entry.typeId))
                TypeOfExercises.Add(entry.typeId, entry.prefab);
        }

        timeSubsystem = new TimeSubsystem();

        DontDestroyOnLoad(gameObject); // no se destruye al cambiar de escena
    }

    private IEnumerator InitExerciseSystem()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
    yield return LoadStreamingAsset("data_exercises.json", (data) =>
    {
        if (data != null)
        {
            string json = System.Text.Encoding.UTF8.GetString(data);
            exercisesSystem = new ExercisesSystem(json, true); // raw JSON
            Debug.Log("Exercises loaded (WebGL)");
        }
        else
        {
            Debug.LogError("Failed to load data_exercises.json in WebGL");
        }
    });
#else
        string path = Path.Combine(Application.streamingAssetsPath, "data_exercises.json");
        if (File.Exists(path))
        {
            exercisesSystem = new ExercisesSystem(path, false); // ruta de archivo
            Debug.Log("Exercises loaded (Standalone)");
        }
        else
        {
            Debug.LogError("File not found: " + path);
        }
        yield return null;
#endif
    }

    public static IEnumerator LoadStreamingAsset(string relativePath, Action<byte[]> onComplete)
    {
        string url = Application.streamingAssetsPath + "/" + relativePath;

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                onComplete?.Invoke(www.downloadHandler.data);
            }
            else
            {
                Debug.LogError($"Failed to load {url}: {www.error}");
                onComplete?.Invoke(null);
            }
        }
    }

    private void Start()
    {
        timeSubsystem.OnTimeOver+= OnTimeOver;
        exercisesSystem.OnPackageChanged += HandlePackageChanged;    
    }

    private void OnTimeOver()
    {
        bCanTakeObject = false;
        timeSubsystem.Pause();

        DestroyLastExercise();

        if (exercisesSystem.UpdateNextPackage())
        {
            OnExerciseFinished();
        }
        else
        {
            HandlePackageChanged(null);
        }
    }

    void HandlePackageChanged(Package newPackage)
    {
        if(newPackage!=null) timeSubsystem.SetTime(newPackage.time); // Update Timer

        UIManager.Instance.GetExercisePanel()?.gameObject.SetActive(false);

        timeSubsystem.Pause();

        if(answersCounter.errors > 0 || answersCounter.success > 0) 
            onAnswersCounterChanged?.Invoke(answersCounter);


        Debug.Log("STARTING COUNTDOWN");

        //Play sound instruction if exists
        PlayPackageInstruction();


        UIManager.Instance.ShowCountdown(5, () =>
        {
            if (newPackage == null) { LoadScene("GameOver"); return; }

            Debug.Log("¡Comenzamos el paquete " + newPackage.typeId + "!");
            timeSubsystem.Resume();

            UIManager.Instance.GetExercisePanel()?.gameObject.SetActive(true);

            answersCounter = new AnswersCounter(); // Update Score
            onAnswersCounterChanged?.Invoke(answersCounter);
        });
    }

    public void OnExerciseFinished()
    {
        bCanTakeObject = false;

        //Get a random diferentExercise
        if (exercisesSystem == null)
        {
            Debug.LogError("ExercisesSystem no está inicializado.");
            return;
        }

        if (!exercisesSystem.HasPendingExercises())
        {
            Debug.Log("No quedan ejercicios en el paquete actual.");

            if (!exercisesSystem.UpdateNextPackage()) { //FINAL GAME
                DestroyLastExercise();

                return;
            }
        }
        if (exercisesSystem.GetCurrentPackage()==null) return;

        Exercise nextExercise = exercisesSystem.GetNextExercise();
        if (nextExercise == null)
        {
            Debug.Log("No quedan más ejercicios en ningún paquete.");
            return;
        }

        Debug.Log("Siguiente ejercicio: " + nextExercise.id);

        //Generate the exercise
        int typeId = exercisesSystem.GetCurrentPackageTypeId();
        if (TypeOfExercises.ContainsKey(typeId))
        {
            DestroyLastExercise();

            GameObject gmO = Instantiate(TypeOfExercises[typeId], UIManager.Instance.GetExercisePanel());
            currentExercise = gmO.GetComponent<ExerciseBase>();
            if (!currentExercise)
            {
                Debug.LogError("Exersise generated is not an exercise");
                return;
            }

            currentExercise?.Show(nextExercise);
            currentExercise.onExerciseFinished += OnExerciseFinished;
            bCanTakeObject = true;
        }
        else
        {
            Debug.LogWarning($"No hay prefab asignado para el typeId {typeId}");
        }
    }

    void DestroyLastExercise()
    {
        if (currentExercise != null)
        {
            currentExercise.onExerciseFinished -= OnExerciseFinished;
            Destroy(currentExercise.gameObject);
            currentExercise = null;
        }
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
            AudioManager.Instance.PlaySFX("fx_correct",0.8f);
        }
        else
        {
            answersCounter.errors++;
            AudioManager.Instance.PlaySFX("fx_error",0.8f);
        }
        Debug.Log($"Aciertos: {answersCounter.success} - Errores: {answersCounter.errors}");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Escena cargada: {scene.name}");

        if (scene.name == "MainMenu")
        {
            ResetGame();
            AudioManager.Instance.PlayBGM("ms_mainMenuMusic");
        }
        else if (scene.name == "GameLevel")
        {
            AudioManager.Instance.PlayBGM("ms_landscape_1");
            //For Next Exercise
            UIManager.Instance.GetScoreWidget()?.gameObject.SetActive(true);

            OnExerciseFinished(); // Get a new Exercise
        }
        else if (scene.name == "GameOver")
        {
            AudioManager.Instance.PlayBGM("ms_mainMenuMusic");
        }
    }

    public void ResetGame()
    {
        exercisesSystem.Reset();
        timeSubsystem.Reset();

        answersCounter = new AnswersCounter();

        currentExercise?.gameObject?.SetActive(false);

        DestroyLastExercise();
    }
    public void LoadScene(string gameLevel)
    {
        SceneManager.LoadScene(gameLevel);
    }

    public void PlayPackageInstruction()
    {
        //Play sound instruction if exists
        ExerciseEntry entry = TypeOfExercisesList.Find(e => e.typeId == exercisesSystem.GetCurrentPackageTypeId());
        if (entry != null && !string.IsNullOrEmpty(entry.soundInstruction))
        {
            AudioManager.Instance.PlaySFX(entry.soundInstruction,0.75f);
        }
    }

   public void SetCanTakeObject(bool active) { bCanTakeObject = active; }
}

