using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ExercisesSystem
{
    public Root loadedData;

    int currentPackageIndex = -1; // El índice del paquete actual
    List<Exercise> pendingExercises; // Lista de ejercicios pendientes del paquete actual

    Dictionary<int, int> ExercisesDone;

    // Evento: se lanza cuando cambiamos de paquete
    public event Action<Package> OnPackageChanged;

    public ExercisesSystem(){
        ExercisesDone = new Dictionary<int, int>();
    }

    public ExercisesSystem(string DataPath)
    {
        ExercisesDone = new Dictionary<int, int>();
        LoadData(DataPath);
    }

    void LoadData(string DataPath)
    {
        string json = File.ReadAllText(DataPath);
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

    // Devuelve el siguiente ejercicio (sin repetir)
    public Exercise GetNextExercise()
    {
        if (pendingExercises == null || pendingExercises.Count == 0)
        {
            // Pasar al siguiente paquete
            InitPackage(currentPackageIndex + 1);

            if (pendingExercises == null || pendingExercises.Count == 0)
                return null; // no hay más paquetes

            OnPackageChanged?.Invoke(loadedData.packages[currentPackageIndex]);
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
}
