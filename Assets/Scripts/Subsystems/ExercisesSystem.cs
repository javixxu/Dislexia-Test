using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ExercisesSystem
{
    public Root loadedData;

    int currentPackageIndex = 0; // El índice del paquete actual
    List<Exercise> pendingExercises; // Lista de ejercicios pendientes del paquete actual

    Dictionary<int, int> ExercisesDone;

    // Evento: se lanza cuando cambiamos de paquete
    public event Action<Package> OnPackageChanged;

    public ExercisesSystem(){
        ExercisesDone = new Dictionary<int, int>();
    }

    // Ahora recibimos directamente el JSON
    public ExercisesSystem(string json, bool isRawJson = true)
    {
        ExercisesDone = new Dictionary<int, int>();

        if (isRawJson)
        {
            LoadFromJson(json);
        }
        else
        {
            // Para compatibilidad en Standalone
            LoadFromFile(json);
        }
    }

    private void LoadFromJson(string json)
    {
        loadedData = JsonUtility.FromJson<Root>(json);
    }

    private void LoadFromFile(string path)
    {
        string json = File.ReadAllText(path);
        loadedData = JsonUtility.FromJson<Root>(json);
    }

    void InitPackage(int packageIndex)
    {
        if (loadedData == null || loadedData.packages == null || packageIndex >= loadedData.packages.Count)
        {
            pendingExercises = null;
            return;
        }

        currentPackageIndex = packageIndex;
        pendingExercises = new List<Exercise>(loadedData.packages[packageIndex].exercises);
    }

    public void Reset()
    {
        currentPackageIndex = -1;
        pendingExercises = null;
        ExercisesDone.Clear();
    }

    // Devuelve el siguiente ejercicio (sin repetir)
    public Exercise GetNextExercise()
    {
        if (pendingExercises == null || pendingExercises.Count == 0)
        {
            return null;
        }

        int randomIndex = UnityEngine.Random.Range(0, pendingExercises.Count);
        Exercise exercise = pendingExercises[randomIndex];
        pendingExercises.RemoveAt(randomIndex);

        // Actualizar contador
        int typeId = loadedData.packages[currentPackageIndex].typeId;
        if (ExercisesDone.ContainsKey(typeId))
            ExercisesDone[typeId]++;
        else
            ExercisesDone[typeId] = 1;

        return exercise;
    }

    // Devuelve cuántos ejercicios se han hecho de un tipo
    public int GetExercisesDoneByType(int typeId)
    {
        if (ExercisesDone.ContainsKey(typeId))
            return ExercisesDone[typeId];
        return 0;
    }

    public int GetCurrentPackageIndex() { return currentPackageIndex; }

    public int GetCurrentPackageTypeId()
    {
        if (loadedData == null || loadedData.packages == null || currentPackageIndex >= loadedData.packages.Count)
            return -1;

        return loadedData.packages[currentPackageIndex].typeId;
    }

    public bool UpdateNextPackage()
    {
        if (loadedData == null || loadedData.packages == null || loadedData.packages.Count == 0)
        {
            Debug.LogWarning("No hay paquetes cargados.");
            return false;
        }

        // Avanzar al siguiente paquete
        currentPackageIndex++;

        if (currentPackageIndex >= loadedData.packages.Count)
        {
            Debug.Log("No hay más paquetes disponibles.");
            pendingExercises = null;
            return false;
        }

        // Inicializar ejercicios del nuevo paquete
        InitPackage(currentPackageIndex);

        // Lanzar evento indicando el paquete actual
        OnPackageChanged?.Invoke(loadedData.packages[currentPackageIndex]);
        return true;
    }

    public Package GetCurrentPackage()
    {
        if (loadedData == null || loadedData.packages == null || currentPackageIndex < 0 || currentPackageIndex >= loadedData.packages.Count)
            return null;

        return loadedData.packages[currentPackageIndex];
    }

    public bool HasPendingExercises()
    {
        return pendingExercises != null && pendingExercises.Count > 0;
    }
}
